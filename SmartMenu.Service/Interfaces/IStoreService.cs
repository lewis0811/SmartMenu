using SmartMenu.Domain.Models.DTO;
using SmartMenu.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartMenu.Service.Interfaces
{
    public interface IStoreService
    {
        IEnumerable<Store> GetAll(int? storeId, int? brandId, string? searchString, int pageNumber, int pageSize);
        IEnumerable<Store> GetStoreWithMenus(int? storeId, int? brandId, string? searchString, int pageNumber, int pageSize);
        IEnumerable<Store> GetStoreWithCollections(int? storeId, int? brandId, string? searchString, int pageNumber, int pageSize);
        Store Add(StoreCreateDTO storeCreateDTO);
        Store Update(int storeId, StoreUpdateDTO storeUpdateDTO);
        void Delete(int storeId);
        Store GetStoreWithStaffs(int storeId, Guid userId, string? searchString, int pageNumber, int pageSize);
    }
}
