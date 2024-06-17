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

        public IEnumerable<ProductGroupItem> GetAll(int? productGroupItemId, int? productGroupId, int? productSizePriceId, string? searchString, int pageNumber = 1, int pageSize = 10)
        {
            var data = _context.ProductGroupsItem
                .Include(c => c.ProductSizePrice)
                .ThenInclude(c => c!.Product)
                .AsQueryable();

            return DataQuery(data, productGroupItemId, productGroupId, productSizePriceId, searchString, pageNumber, pageSize);
        }

        private IEnumerable<ProductGroupItem> DataQuery(IQueryable<ProductGroupItem> data, int? productGroupItemId, int? productGroupId, int? productSizePriceId, string? searchString, int pageNumber, int pageSize)
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
            if (productSizePriceId != null)
            {
                data = data
                    .Where(c => c.ProductSizePriceId == productSizePriceId);
            }

            if (searchString != null)
            {
                data = data
                    .Where(c => c.ProductSizePrice!.Price.ToString() == searchString
                    || c.ProductSizePrice.Product!.ProductName == searchString);
            }

            return PaginatedList<ProductGroupItem>.Create(data, pageNumber, pageSize);
        }
    }
}