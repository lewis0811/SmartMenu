using Microsoft.EntityFrameworkCore;
using SmartMenu.Domain.Models;
using SmartMenu.Domain.Repository;

namespace SmartMenu.DAO.Implementation
{
    public class StoreProductRepository : GenericRepository<StoreProduct>, IStoreProductRepository
    {
        private readonly SmartMenuDBContext _context;

        public StoreProductRepository(SmartMenuDBContext context) : base(context)
        {
            _context = context;
        }

        public IEnumerable<StoreProduct> GetStoreProducts(int? storeProductId, string? searchString, int pageNumber, int pageSize)
        {
            var data = _context.StoreProducts
                             .Include(x => x.Product)
                             .AsQueryable();
            return DataQuery(data, storeProductId, searchString, pageNumber, pageSize);
        }

        private IEnumerable<StoreProduct> DataQuery(IQueryable<StoreProduct> data, int? storeProductId, string? searchString, int pageNumber, int pageSize)
        {
            data = data.Where(c => c.IsDeleted == false);
            if (storeProductId != null)
            {
                data = data
                    .Where(c => c.StoreProductId == storeProductId);
            }

            if (searchString != null)
            {
                searchString = searchString.Trim();
                data = data
                    .Where(c => c.Product!.ProductName.Contains(searchString));
            }

            return PaginatedList<StoreProduct>.Create(data, pageNumber, pageSize);
        }
    }
}