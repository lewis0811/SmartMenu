using Microsoft.EntityFrameworkCore;
using SmartMenu.Domain.Models;

using SmartMenu.Domain.Repository;

namespace SmartMenu.DAO.Implementation
{
    public class ProductSizePriceRepository : GenericRepository<ProductSizePrice>, IProductSizePriceRepository
    {
        private readonly SmartMenuDBContext _context;

        public ProductSizePriceRepository(SmartMenuDBContext context) : base(context)
        {
            _context = context;
        }

        public IEnumerable<ProductSizePrice> GetAll(int? productSizePriceId, int? productId, string? searchString, int pageNumber, int pageSize)
        {
            var data = _context.ProductSizePrices
                .AsQueryable();

            return DataQuery(data, productSizePriceId, productId, searchString, pageNumber, pageSize);
        }

        private IEnumerable<ProductSizePrice> DataQuery(IQueryable<ProductSizePrice> data, int? productSizePriceId, int? productId, string? searchString, int pageNumber, int pageSize)
        {
            data = data.Where(c => c.IsDeleted == false);

            if (productSizePriceId != null)
            {
                data = data.Where(c => c.ProductSizePriceId == productSizePriceId);
            }

            if (productId != null)
            {
                data = data.Where(c => c.ProductId == productId);
            }

            if (searchString != null)
            {
                data = data.Where(c => c.ProductSizeType.ToString().Contains(searchString)
                || c.Price.ToString() == searchString);
            }

            return PaginatedList<ProductSizePrice>.Create(data, pageNumber, pageSize);
        }
    }
}