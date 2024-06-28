using SmartMenu.Domain.Models;

namespace SmartMenu.Domain.Repository
{
    public interface ICollectionRepository : IGenericRepository<Models.Collection>
    {
        public IEnumerable<Models.Collection> GetAll(int? collectionId, int? brandId, string? searchString, int pageNumber = 1, int pageSize = 10);

        public IEnumerable<Models.Collection> GetCollectionWithProductGroup(int? collectionId, int? brandId, string? searchString, int pageNumber = 1, int pageSize = 10);
    }
}
