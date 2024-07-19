using SmartMenu.Domain.Models;
using SmartMenu.Domain.Models.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartMenu.Service.Interfaces
{
    public interface IStoreProductService
    {
        IEnumerable<StoreProduct> GetAll(int? storeProductId, int? storeId, int? productId, string? searchString, int pageNumber, int pageSize);
        StoreProduct Add(StoreProductCreateDTO storeProductCreateDTO);
        StoreProduct Update(int storeProductId, StoreProductUpdateDTO storeProductUpdateDTO);
        void Delete(int storeProductId);
        IEnumerable<StoreProduct> GetWithProductSizePrices(int? storeProductId, int? storeId, int? productId, string? searchString, int pageNumber, int pageSize);
        List<StoreProduct> AddV2(StoreProductCreateDTO_V2 storeProductCreateDTO);
    }
}
