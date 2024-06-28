using SmartMenu.Domain.Models;

namespace SmartMenu.Domain.Repository
{
    public interface IProductGroupRepository : IGenericRepository<ProductGroup>
    {
        public IEnumerable<ProductGroup> GetAll(int? productGroupId, int? menuId, int? collectionId, string? searchString, int pageNumber = 1, int pageSize = 10);

        public IEnumerable<ProductGroup> GetProductGroupWithGroupItem(int? productGroupId, int? menuId, int? collectionId, string? searchString, int pageNumber = 1, int pageSize = 10);
        public List<ProductGroup> GetProductGroup(int? productGroupId, int? menuId, int? collectionId);
    }
}
