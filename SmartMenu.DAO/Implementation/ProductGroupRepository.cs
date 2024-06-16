using Microsoft.EntityFrameworkCore;
using SmartMenu.Domain.Models;
using SmartMenu.Domain.Repository;

namespace SmartMenu.DAO.Implementation
{
    public class ProductGroupRepository : GenericRepository<ProductGroup>, IProductGroupRepository
    {
        private readonly SmartMenuDBContext _context;

        public ProductGroupRepository(SmartMenuDBContext context) : base(context)
        {
            _context = context;
        }

        public IEnumerable<ProductGroup> GetAll(int? productGroupId, int? menuId, int? collectionId, string? searchString, int pageNumber = 1, int pageSize = 10)
        {
            var data = _context.ProductGroups.AsQueryable();

            return DataQuery(data, productGroupId, menuId, collectionId, searchString, pageNumber, pageSize);
        }

        public IEnumerable<ProductGroup> GetProductGroupWithGroupItem(int? productGroupId, int? menuId, int? collectionId, string? searchString, int pageNumber = 1, int pageSize = 10)
        {
            var data = _context.ProductGroups
                .Include(c => c.ProductGroupItems)!
                .ThenInclude(c => c.ProductSizePrice)
                .ThenInclude(c => c.Product)
                .AsQueryable();

            return DataQuery(data, productGroupId, menuId, collectionId, searchString, pageNumber, pageSize);
        }
        private IEnumerable<ProductGroup> DataQuery(IQueryable<ProductGroup> data, int? productGroupId, int? menuId, int? collectionId, string? searchString, int pageNumber, int pageSize)
        {
            data = data.Where(c => c.IsDeleted == false);
            if (productGroupId != null)
            {
                data = data
                    .Where(c => c.ProductGroupID == productGroupId);
            }

            if (menuId != null)
            {
                data = data
                    .Where(c => c.MenuID == menuId);
            }
            if (collectionId != null)
            {
                data = data
                    .Where(c => c.CollectionID == collectionId);
            }

            if (searchString != null)
            {
                searchString = searchString.Trim();
                data = data
                    .Where(c => c.ProductGroupName.Contains(searchString));
            }

            return PaginatedList<ProductGroup>.Create(data, pageNumber, pageSize);
        }
    }
}
