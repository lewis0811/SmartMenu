using SmartMenu.Domain.Models;
using SmartMenu.Domain.Models.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartMenu.Service.Interfaces
{
    public interface IAuthService
    {
        User Find(string gmail);
        Task<Guid?> ForgotPassword(string email);
        object Login(UserLoginDTO userLoginDTO);
        void Register(UserCreateDTO userCreateDTO);
        bool ResetPasswordAsync(User user, string token, string password);
        void VerifyEmail(string email);
    }
}
