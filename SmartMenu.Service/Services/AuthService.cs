﻿using AutoMapper;
using Azure.Core;
using CloudinaryDotNet;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json.Linq;
using SmartMenu.Domain.Models;
using SmartMenu.Domain.Models.DTO;
using SmartMenu.Domain.Models.Enum;
using SmartMenu.Domain.Repository;
using SmartMenu.Service.Interfaces;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace SmartMenu.Service.Services
{
    public class AuthService : IAuthService
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly string secretKey;

        public AuthService(IMapper mapper, IUnitOfWork unitOfWork, IConfiguration configuration)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            secretKey = configuration["ApiSettings:Secret"];
        }

        public User Find(string gmail)
        {
            var data = _unitOfWork.UserRepository.Find(c => c.Email == gmail && c.IsDeleted == false)
                .FirstOrDefault() ?? throw new Exception("User not found or deleted");
            return data;
        }

        public object Login(UserLoginDTO userLoginDTO)
        {
            var data = _unitOfWork.UserRepository.Login(userLoginDTO);
            if (data == null) throw new Exception("Username or password  incorrect");

            var role = data.Role!;

            // Generate JWT token
            JwtSecurityTokenHandler handler = new();
            byte[] key = Encoding.ASCII.GetBytes(secretKey);

            SecurityTokenDescriptor securityTokenDescriptor = new()
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim("Id", data.UserId.ToString()),
                    new Claim("Username", data.UserName),
                    new Claim(ClaimTypes.Email, data.Email),
                    new Claim(ClaimTypes.Role, Enum.GetName(typeof(Role),role)!.ToString())
                }),
                Expires = DateTime.UtcNow.AddDays(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            SecurityToken token = handler.CreateToken(securityTokenDescriptor);
            string tokenString = handler.WriteToken(token);

            // Get brand & staff id for user
            int? brandId = null;
            int? storeId = null;
            int? staffId = null;
            var brandStaff = _unitOfWork.BrandStaffRepository.Find(c => c.UserId == data.UserId)
                .FirstOrDefault();

            if (brandStaff != null)
            {
                brandId = brandStaff.BrandId;
                staffId = brandStaff.BrandStaffId;
                storeId = brandStaff.StoreId;
            }

            return new
            {
                UserId = data.UserId,
                RoleId = data.Role,
                Role = data.Role.ToString(),
                BrandId = brandId,
                BrandStaffId = staffId,
                StoreId = storeId,
                Token = tokenString
            };
        }

        public void Register(UserCreateDTO userCreateDTO)
        {
            var data = _mapper.Map<User>(userCreateDTO);
            _unitOfWork.UserRepository.Add(data);
            _unitOfWork.Save();
        }

        public async Task<Guid?> ForgotPassword(string email)
        {
            var user = await _unitOfWork.UserRepository.FindObjectAsync(c => c.Email == email && c.IsDeleted == false)
                ?? throw new Exception("User not found or deleted");

            user.Token = Guid.NewGuid();
            _unitOfWork.UserRepository.Update(user);
            _unitOfWork.Save();

            return user.Token;
        }

        public bool ResetPasswordAsync(User user, string token, string password)
        {
            try
            {
                if (user.Token != Guid.Parse(token)) throw new Exception("Invalid token");

                user.Password = password;
                user.Token = null;
                _unitOfWork.UserRepository.Update(user);
                _unitOfWork.Save();

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}