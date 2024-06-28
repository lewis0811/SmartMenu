using Microsoft.EntityFrameworkCore;
using SmartMenu.Domain.Models;
using SmartMenu.Domain.Repository;

namespace SmartMenu.DAO.Implementation
{
    public class ProductGroupItemRepository : GenericRepository<ProductGroupItem>, IProductGroupItemRepository
    {
        private readonly SmartMenuDBContext _context;

        public ProductGroupItemRepository(SmartMenuDBContext context) : base(context)
        {
            _context = context;
        }

        public IEnumerable<ProductGroupItem> GetAll(int? productGroupItemId, int? productGroupId, int? productId, string? searchString, int pageNumber = 1, int pageSize = 10)
        {
            var data = _context.ProductGroupsItem
                .Include(c => c.Product)
                .ThenInclude(c => c!.ProductSizePrices)
                .Include(c => c!.Product)
                .AsQueryable();

            return DataQuery(data, productGroupItemId, productGroupId, productId, searchString, pageNumber, pageSize);
        }

        private IEnumerable<ProductGroupItem> DataQuery(IQueryable<ProductGroupItem> data, int? productGroupItemId, int? productGroupId, int? productId, string? searchString, int pageNumber, int pageSize)
        {
            data = data.Where(c => c.IsDeleted == false);
            if (productGroupItemId != null)
            {
                data = data
                    .Where(c => c.ProductGroupId == productGroupItemId);
            }

            if (productGroupId != null)
            {
                data = data
                    .Where(c => c.ProductGroupId == productGroupId);
            }
            if (productId != null)
            {
                data = data
                    .Where(c => c.ProductId == productId);
            }

            if (searchString != null)
            {
                data = data
                    .Where(c => c.Product!.ProductName.Contains(searchString)
                    || c.Product.ProductSizePrices!.Any(d => d.Price.ToString().Contains(searchString))
                    || c.Product.ProductSizePrices!.Any(d => d.ProductSizeType.ToString().Contains(searchString))
                    );
            }

            return PaginatedList<ProductGroupItem>.Create(data, pageNumber, pageSize);
        }
    }
}