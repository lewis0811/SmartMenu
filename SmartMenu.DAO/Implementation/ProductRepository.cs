using Microsoft.EntityFrameworkCore;
using SmartMenu.Domain.Models;
using SmartMenu.Domain.Repository;


namespace SmartMenu.DAO.Implementation
{
    public class ProductRepository : GenericRepository<Product>, IProductRepository
    {
        private readonly SmartMenuDBContext _context;

        public ProductRepository(SmartMenuDBContext context) : base(context)
        {
            _context = context;
        }

        public IEnumerable<Product> GetAll(int? productId, int? brandId, int? categoryId, string? searchString, int pageNumber = 1, int pageSize = 10)
        {
            var data = _context.Products
                .Include(c => c.ProductSizePrices)!
                .ThenInclude(c => c.ProductSize)
                .AsQueryable();
            return DataQuery(data, productId, brandId, categoryId, searchString, pageNumber, pageSize);
        }

        private IEnumerable<Product> DataQuery(IQueryable<Product> data, int? productId, int? brandId, int? categoryId, string? searchString, int pageNumber, int pageSize)
        {
            data = data.Where(c => c.IsDeleted == false);
            if (productId != null)
            {
                data = data
                    .Where(c => c.ProductID == productId);
            }

            if (brandId != null)
            {
                data = data
                    .Where(c => c.BrandID == brandId);
            }

            if (categoryId != null)
            {
                data = data
                    .Where(c => c.CategoryID == categoryId);
            }

            if (searchString != null)
            {
                searchString = searchString.Trim();
                data = data
                    .Where(c => c.ProductName.Contains(searchString)
                    || c.ProductDescription.Contains(searchString));
            }

            return PaginatedList<Product>.Create(data, pageNumber, pageSize);
        }
    }
}