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
        object Login(UserLoginDTO userLoginDTO);
        void Register(UserCreateDTO userCreateDTO);
    }
}
