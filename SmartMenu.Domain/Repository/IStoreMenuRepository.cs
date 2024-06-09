using SmartMenu.Domain.Models;

namespace SmartMenu.Domain.Repository
{
    public interface IStoreMenuRepository : IGenericRepository<StoreMenu>
    {
        public IEnumerable<StoreMenu> GetAll(int? storeMenuId, int? storeId, int? menuId, string? searchString, int pageNumber = 1, int pageSize = 10);
    }
}
