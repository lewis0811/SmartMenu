using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartMenu.Domain.Models.DTO;
using SmartMenu.Service.Interfaces;

namespace SmartMenu.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly IEmailService _emailService;
        private readonly IMapper _mapper;
        private readonly string secretKey;

        public AuthController(IAuthService authService, IConfiguration configuration, IMapper mapper, IEmailService emailService)
        {
            _authService = authService;
            secretKey = configuration.GetValue<string>("ApiSettings:Secret");
            _mapper = mapper;
            _emailService = emailService;
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
                return BadRequest(new { error = ex.Message });
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
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpPost("ForgotPassword")]
        [AllowAnonymous]
        public async Task<IActionResult> ForgotPassword(string email)
        {
            try
            {
                var token = await _authService.ForgotPassword(email);
                var user = _authService.Find(email) ?? throw new Exception("User not found or deleted");

                var resetLink = Url.Action(nameof(ResetPassword), "Auth", new { token, email = user.Email }, Request.Scheme);

                string emailBody = $@"
                    <!DOCTYPE html>
                    <html>
                        <head>
                            <title>Reset Your Password</title>
                        </head>
                        <body>
                            <p>Hi {user.UserName},</p>

                            <p>You recently requested to reset your password for your account. Please click the button below to reset it:</p>

                            <a href=""{resetLink}"" style=""display: inline-block; padding: 10px 20px; background-color: #007bff; color: #fff; text-decoration: none; border-radius: 5px;"">Reset Password</a>

                            <p>If you didn't request a password reset, you can safely ignore this email.</p>

                            <p>Thanks,<br>
                            SmartMenu Team</p>
                        </body>
                    </html>
                ";

                _emailService.SendEmail(new MessageCreateDTO(new string[] { user.Email }, "Reset Password", emailBody));
                return Ok("Reset password email sent successfully to " + user.Email);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpGet("ResetPassword")]
        public IActionResult ResetPassword(string token, string email)
        {
            if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development")
            {
                // Localhost redirect for development
                return Redirect($"http://localhost:3000/pages/reset-password?token={token}&email={email}");
            }
            else
            {
                // Production redirect to smartmenuweb.netlify.app
                return Redirect($"https://smartmenuweb.netlify.app/pages/reset-password?token={token}&email={email}");
            }
        }

        [HttpPost("ResetPassword")]
        public IActionResult ResetPassword(UserResetPasswordDTO userResetPasswordDTO)
        {
            try
            {
                var user = _authService.Find(userResetPasswordDTO.Email) ?? throw new Exception("User does not exist");

                var result = _authService.ResetPasswordAsync(user, userResetPasswordDTO.Token, userResetPasswordDTO.Password);
                if (!result) throw new Exception("Failed to reset password");

                return Ok("Password has been changed successfully");
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }
    }
}