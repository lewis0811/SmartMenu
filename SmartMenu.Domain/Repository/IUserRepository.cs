﻿using SmartMenu.Domain.Models;
using SmartMenu.Domain.Models.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartMenu.Domain.Repository
{
    public interface IUserRepository : IGenericRepository<User>
    {
        User Login(UserLoginDTO userLoginDTO);
    }
}
