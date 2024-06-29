using SmartMenu.Domain.Models;
using SmartMenu.Domain.Models.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartMenu.Service.Interfaces
{
    public interface IStoreMenuService
    {
        IEnumerable<StoreMenu> GetAll(int? storeMenuId, int? storeId, int? menuId, string? searchString, int pageNumber, int pageSize);
        StoreMenu Add(StoreMenuCreateDTO storeMenuCreateDTO);
        StoreMenu Update(int storeMenuId, StoreMenuCreateDTO storeMenuCreateDTO);
        void Delete(int storeMenuId);
    }
}
