using SmartMenu.Domain.Models;

namespace SmartMenu.Domain.Repository
{
    public interface IProductGroupItemRepository : IGenericRepository<ProductGroupItem>
    {
        public IEnumerable<ProductGroupItem> GetAll(int? productGroupItemId, int? productGroupId, int? productId, string? searchString, int pageNumber = 1, int pageSize = 10);
    }
}
