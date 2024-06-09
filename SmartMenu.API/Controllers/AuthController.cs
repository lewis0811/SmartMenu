using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using SmartMenu.Domain.Models;
using SmartMenu.Domain.Models.DTO;
using SmartMenu.Domain.Repository;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace SmartMenu.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly string secretKey;

        public AuthController(IUnitOfWork unitOfWork, IConfiguration configuration, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            this.secretKey = configuration.GetValue<string>("ApiSettings:Secret");
            _mapper = mapper;
        }

        [HttpPost("Register")]
        public IActionResult Register(UserCreateDTO userCreateDTO)
        {
            var data = _mapper.Map<User>(userCreateDTO);
            _unitOfWork.UserRepository.Add(data);
            _unitOfWork.Save();
            return Ok(userCreateDTO);
        }

        [HttpPost("Login")]
        public IActionResult Login(UserLoginDTO userLoginDTO)
        {
            var data = _unitOfWork.UserRepository.Login(userLoginDTO);
            if (data == null) return BadRequest("Username or password  incorrect");

            // Generate JWT token
            var role = _unitOfWork.RoleRepository.GetByID(data.RoleID);
            JwtSecurityTokenHandler handler = new();
            byte[] key = Encoding.ASCII.GetBytes(secretKey);

            SecurityTokenDescriptor securityTokenDescriptor = new()
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim("Id", data.UserID.ToString()),
                    new Claim("Username", data.UserName),
                    new Claim(ClaimTypes.Email, data.Email),
                    new Claim(ClaimTypes.Role, role.RoleName)
                }),
                Expires = DateTime.UtcNow.AddDays(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            SecurityToken token = handler.CreateToken(securityTokenDescriptor);
            string tokenString = handler.WriteToken(token);

            return Ok(new { token = tokenString });
        }
    }
}