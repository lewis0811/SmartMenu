using SmartMenu.Domain.Models;

namespace SmartMenu.Domain.Repository
{
    public interface ICategoryRepository : IGenericRepository<Category>
    {
        public IEnumerable<Category> GetAll(int? categoryId, string? searchString, int pageNumber = 1, int pageSize = 10);

    }
}
