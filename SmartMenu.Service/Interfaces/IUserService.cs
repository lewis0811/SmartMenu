using SmartMenu.Domain.Models;
using SmartMenu.Domain.Models.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartMenu.Service.Interfaces
{
    public interface IUserService
    {
        void Delete(Guid userId);
        IEnumerable<User> GetAll(Guid? userId, bool isDeleted, string? searchString, int pageNumber, int pageSize);
        User Update(Guid userId, UserUpdateDTO userUpdateDTO);
    }
}
