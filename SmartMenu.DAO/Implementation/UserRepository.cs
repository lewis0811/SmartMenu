using SmartMenu.Domain.Models;
using SmartMenu.Domain.Models.DTO;
using SmartMenu.Domain.Repository;

namespace SmartMenu.DAO.Implementation
{
    public class UserRepository : GenericRepository<User>, IUserRepository
    {
        private readonly SmartMenuDBContext _context;
        public UserRepository(SmartMenuDBContext context) : base(context)
        {
            _context = context;
        }

        public User Login(UserLoginDTO userLoginDTO)
        {
            User user = _context.Users
                .FirstOrDefault(c
                => c.UserName.ToLower().Equals(userLoginDTO.UserName.ToLower())
                && c.Password.Equals(userLoginDTO.Password))!;

            return user;
        }
    }
}
