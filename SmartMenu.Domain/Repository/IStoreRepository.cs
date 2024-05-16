using SmartMenu.Domain.Models;

namespace SmartMenu.Domain.Repository
{
    public interface IStoreRepository : IGenericRepository<Store>
    {
        public IEnumerable<Store> GetAll(int? storeId, int? brandId, string? searchString, int pageNumber = 1, int pageSize = 10);

        public IEnumerable<Store> GetStoreWithMenus(int? storeId, int? brandId, string? searchString, int pageNumber = 1, int pageSize = 10);

        public IEnumerable<Store> GetStoreWithCollections(int? storeId, int? brandId, string? searchString, int pageNumber = 1, int pageSize = 10);
    }
}