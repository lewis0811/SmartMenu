using Microsoft.EntityFrameworkCore;
using SmartMenu.Domain.Models;
using SmartMenu.Domain.Repository;

namespace SmartMenu.DAO.Implementation
{
    public class StoreRepository : GenericRepository<Store>, IStoreRepository
    {
        private readonly SmartMenuDBContext _context;

        public StoreRepository(SmartMenuDBContext context) : base(context)
        {
            _context = context;
        }

        public IEnumerable<Store> GetAll(int? storeId, int? brandId, string? searchString, int pageNumber, int pageSize)
        {
            var data = _context.Stores.AsQueryable();

            return DataQuery(data, storeId, brandId, searchString, pageNumber, pageSize);
        }

        public IEnumerable<Store> GetStoreWithMenus(int? storeId, int? brandId, string? searchString, int pageNumber, int pageSize)
        {
            var data = _context.Stores
                .Include(x => x.StoreMenus)!
                .ThenInclude(c => c.Menu)
                .AsQueryable();

            return DataQuery(data, storeId, brandId, searchString, pageNumber, pageSize);
        }

        public IEnumerable<Store> GetStoreWithCollections(int? storeId, int? brandId, string? searchString, int pageNumber, int pageSize)
        {
            var data = _context.Stores
                .Include(c => c.StoreCollections)!
                .ThenInclude(c => c.Collection)
                .AsQueryable();

            return DataQuery(data, storeId, brandId, searchString, pageNumber, pageSize);
        }

        private IEnumerable<Store> DataQuery(IQueryable<Store> data, int? storeId, int? brandId, string? searchString, int pageNumber, int pageSize)
        {
            data = data.Where(c => c.IsDeleted == false);
            if (storeId != null)
            {
                data = data
                    .Where(c => c.StoreID == storeId);
            }

            if (brandId != null)
            {
                data = data
                    .Where(c => c.BrandID == brandId);
            }

            if (searchString != null)
            {
                searchString = searchString.Trim();
                data = data
                    .Where(c => c.StoreLocation.Contains(searchString)
                    || c.StoreContactEmail.Contains(searchString)
                    || c.StoreContactNumber.Contains(searchString));
            }

            return PaginatedList<Store>.Create(data, pageNumber, pageSize);
        }
    }
}