using SmartMenu.Domain.Models;
using SmartMenu.Domain.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartMenu.DAO.Implementation
{
    public class RoleRepository : GenericRepository<Role>, IRoleRepository
    {
        public RoleRepository(SmartMenuDBContext context) : base(context) { }
    }
}
