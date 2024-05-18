using SmartMenu.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartMenu.Domain.Repository
{
    public interface IStoreMenuRepository : IGenericRepository<StoreMenu>
    {
        public IEnumerable<StoreMenu> GetAll(int? storeMenuId, int? storeId, int? menuId, string? searchString, int pageNumber = 1, int pageSize = 10);
    }
}
