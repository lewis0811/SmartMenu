using SmartMenu.Domain.Models;
using SmartMenu.Domain.Models.DTO;

namespace SmartMenu.Domain.Repository
{
    public interface IUserRepository : IGenericRepository<User>
    {
        User Login(UserLoginDTO userLoginDTO);
    }
}
