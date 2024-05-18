using SmartMenu.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartMenu.Domain.Repository
{
    public interface IMenuRepository : IGenericRepository<Menu>
    {
        public IEnumerable<Menu> GetAll(int? menuId, int? brandId, string? searchString, int pageNumber = 1, int pageSize = 10);

        public IEnumerable<Menu> GetMenuWithProductGroup(int? menuId, int? brandId, string? searchString, int pageNumber = 1, int pageSize = 10);
    }
}
