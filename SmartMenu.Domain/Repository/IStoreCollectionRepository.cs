using SmartMenu.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartMenu.Domain.Repository
{
    public interface IStoreCollectionRepository : IGenericRepository<StoreCollection>
    {
        public IEnumerable<StoreCollection> GetAll(int? storeCollectionId, int? storeId, int? collectionId, string? searchString, int pageNumber = 1, int pageSize = 10);
    }
}
