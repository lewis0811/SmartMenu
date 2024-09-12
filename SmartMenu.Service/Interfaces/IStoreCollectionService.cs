using SmartMenu.Domain.Models.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SmartMenu.Domain.Models;
using System.Threading.Tasks;

namespace SmartMenu.Service.Interfaces
{
    public interface IStoreCollectionService
    {
        IEnumerable<StoreCollection> GetAll(int? storeCollectionId, int? storeId, int? collectionId, string? searchString, int pageNumber, int pageSize);
        StoreCollection Add(StoreCollectionCreateDTO storeCollectionCreateDTO);
        StoreCollection Update(int storeCollectionId, StoreCollectionCreateDTO storeCollectionUpdateDTO);
        void Delete(int storeCollectionId);
        void DeleteV2(int storeCollectionId);
    }
}
