using SmartMenu.Domain.Models;
using SmartMenu.Domain.Repository;

namespace SmartMenu.DAO.Implementation
{
    public class StoreCollectionRepository : GenericRepository<StoreCollection>, IStoreCollectionRepository
    {
        private readonly SmartMenuDBContext _context;

        public StoreCollectionRepository(SmartMenuDBContext context) : base(context)
        {
            _context = context;
        }
        public IEnumerable<StoreCollection> GetAll(int? storeCollectionId, int? storeId, int? collectionId, string? searchString, int pageNumber = 1, int pageSize = 10)
        {
            var data = _context.StoreCollections.AsQueryable();

            return DataQuery(data, storeCollectionId, storeId, collectionId, searchString, pageNumber, pageSize);
        }
        private IEnumerable<StoreCollection> DataQuery(IQueryable<StoreCollection> data, int? storeCollectionId, int? storeId, int? collectionId, string? searchString, int pageNumber, int pageSize)
        {
            data = data.Where(c => c.IsDeleted == false);
            if (storeCollectionId != null)
            {
                data = data
                    .Where(c => c.StoreCollectionID == storeCollectionId);
            }

            if (storeId != null)
            {
                data = data
                    .Where(c => c.StoreID == storeId);
            }
            if (collectionId != null)
            {
                data = data
                    .Where(c => c.CollectionID == collectionId);
            }


            return PaginatedList<StoreCollection>.Create(data, pageNumber, pageSize);
        }
    }
}
