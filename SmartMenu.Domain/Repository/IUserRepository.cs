﻿using SmartMenu.Domain.Models;
using SmartMenu.Domain.Models.DTO;

namespace SmartMenu.Domain.Repository
{
    public interface IUserRepository : IGenericRepository<User>
    {
        //void Delete(Guid userId);
        //IEnumerable<User> GetAll(Guid? userId, string? searchString, int pageNumber, int pageSize);
        //User Login(UserLoginDTO userLoginDTO);
        User Login(UserLoginDTO userLoginDTO);
    }
}
