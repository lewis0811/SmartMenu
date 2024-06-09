using SmartMenu.Domain.Models;

namespace SmartMenu.Domain.Repository
{
    public interface IStoreCollectionRepository : IGenericRepository<StoreCollection>
    {
        public IEnumerable<StoreCollection> GetAll(int? storeCollectionId, int? storeId, int? collectionId, string? searchString, int pageNumber = 1, int pageSize = 10);
    }
}
