using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using SmartMenu.Domain.Models;
using SmartMenu.Domain.Models.DTO;
using SmartMenu.Domain.Models.Enum;
using SmartMenu.Service.Interfaces;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace SmartMenu.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly IMapper _mapper;
        private readonly string secretKey;

        public AuthController(IAuthService authService, IConfiguration configuration, IMapper mapper)
        {
            _authService = authService;
            secretKey = configuration.GetValue<string>("ApiSettings:Secret");
            _mapper = mapper;
        }

        [HttpPost("Register")]
        public IActionResult Register(UserCreateDTO userCreateDTO)
        {
            try
            {
                _authService.Register(userCreateDTO);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(new {error = ex.Message });
            }
        }

        [HttpPost("Login")]
        public IActionResult Login(UserLoginDTO userLoginDTO)
        {
            try
            {
                var data = _authService.Login(userLoginDTO);
                return Ok(data);
            }
            catch (Exception ex)
            {
                return BadRequest(new {error = ex.Message });
            }
        }

    }
}