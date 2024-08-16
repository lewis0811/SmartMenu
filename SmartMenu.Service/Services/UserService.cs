using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SmartMenu.DAO;
using SmartMenu.Domain.Models;
using SmartMenu.Domain.Models.DTO;
using SmartMenu.Domain.Repository;
using SmartMenu.Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartMenu.Service.Services
{
    public class UserService : IUserService
    {

        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public UserService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }


        public IEnumerable<User> GetAll(Guid? userId, bool isDeleted, string? searchString, int pageNumber, int pageSize)
        {
            var data = _unitOfWork.UserRepository.EnableQuery();
            return DataQuery(data, userId, isDeleted, searchString, pageNumber, pageSize);
        }

        public User Update(Guid userId, UserUpdateDTO userUpdateDTO)
        {
            if (userUpdateDTO.Password != userUpdateDTO.ConfirmPassword)
            {
                throw new Exception("Password not match!");
            }
            var data = _unitOfWork.UserRepository.GetByID(userId);
            data.Password = userUpdateDTO.Password;

            _unitOfWork.UserRepository.Update(data);
            _unitOfWork.Save();

            return data;
        }
        public void Delete(Guid userId)
        {
            var data = _unitOfWork.UserRepository.GetByID(userId) ?? throw new Exception("User not found!");
            if (data.Role == Domain.Models.Enum.Role.ADMIN) throw new Exception("Admin can't be deleted!");
            var brandStaff = _unitOfWork.BrandStaffRepository.EnableQuery().FirstOrDefault(c => c.UserId == userId && !c.IsDeleted);
            if (brandStaff != null) throw new Exception($"User are still acvite in brand id {brandStaff.BrandId}");

            data.IsDeleted = true;
            _unitOfWork.UserRepository.Update(data);
            _unitOfWork.Save();
        }

        private static IEnumerable<User> DataQuery(IQueryable<User> data, Guid? userId, bool isDeleted, string? searchString, int pageNumber, int pageSize)
        {
            if (!isDeleted)
            {
                data = data.Where(c => c.IsDeleted == isDeleted);
            }

            if (userId != null)
            {
                data = data
                    .Where(c => c.UserId.ToString() == userId.ToString());
            }

            if (searchString != null)
            {
                searchString = searchString.Trim();
                data = data
                    .Where(c => c.UserName.Contains(searchString)
                    || c.Email.Contains(searchString));
            }

            return PaginatedList<User>.Create(data, pageNumber, pageSize);
        }
    }
}
