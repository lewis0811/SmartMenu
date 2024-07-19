using Microsoft.EntityFrameworkCore;
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

        public IEnumerable<User> GetAll(Guid? userId, string? searchString, int pageNumber, int pageSize)
        {
            var data = _context.Users
                 .AsQueryable();
            return DataQuery(data, userId, searchString, pageNumber, pageSize);
        }

        public User Login(UserLoginDTO userLoginDTO)
        {
            User user = _context.Users
                .FirstOrDefault(c
                => c.UserName.ToLower().Equals(userLoginDTO.UserName.ToLower())
                && c.Password.Equals(userLoginDTO.Password))!;

            return user;
        }

        private static IEnumerable<User> DataQuery(IQueryable<User> data, Guid? userId, string? searchString, int pageNumber, int pageSize)
        {
            data = data.Where(c => c.IsDeleted == false);
   
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
