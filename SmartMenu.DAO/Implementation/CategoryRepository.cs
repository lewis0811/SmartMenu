using SmartMenu.Domain.Models;
using SmartMenu.Domain.Repository;

namespace SmartMenu.DAO.Implementation
{
    public class CategoryRepository : GenericRepository<Category>, ICategoryRepository
    {
        private readonly SmartMenuDBContext _context;
        public CategoryRepository(SmartMenuDBContext context) : base(context)
        {
            _context = context;
        }
        public IEnumerable<Category> GetAll(int? categoryId, string? searchString, int pageNumber = 1, int pageSize = 10)
        {
            var data = _context.Categories.AsQueryable();
            return DataQuery(data, categoryId, searchString, pageNumber, pageSize);
        }
        private IEnumerable<Category> DataQuery(IQueryable<Category> data, int? categoryId, string? searchString, int pageNumber, int pageSize)
        {
            data = data.Where(c => c.IsDeleted == false);
            if (categoryId != null)
            {
                data = data
                    .Where(c => c.CategoryID == categoryId);
            }

            if (searchString != null)
            {
                searchString = searchString.Trim();
                data = data
                    .Where(c => c.CategoryName.Contains(searchString)
);
            }

            return PaginatedList<Category>.Create(data, pageNumber, pageSize);
        }
    }
}
