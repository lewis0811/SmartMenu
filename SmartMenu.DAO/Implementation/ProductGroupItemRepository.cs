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
            var data = _context.ProductGroupsItem.AsQueryable();

            return DataQuery(data, productGroupItemId, productGroupId, productId, searchString, pageNumber, pageSize);
        }
        private IEnumerable<ProductGroupItem> DataQuery(IQueryable<ProductGroupItem> data, int? productGroupItemId, int? productGroupId, int? productId, string? searchString, int pageNumber, int pageSize)
        {
            data = data.Where(c => c.IsDeleted == false);
            if (productGroupItemId != null)
            {
                data = data
                    .Where(c => c.ProductGroupID == productGroupItemId);
            }

            if (productGroupId != null)
            {
                data = data
                    .Where(c => c.ProductGroupID == productGroupId);
            }
            if (productId != null)
            {
                data = data
                    .Where(c => c.ProductID == productId);
            }


            return PaginatedList<ProductGroupItem>.Create(data, pageNumber, pageSize);
        }
    }
}
