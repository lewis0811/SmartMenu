using SmartMenu.Domain.Models;
using SmartMenu.Domain.Repository;

namespace SmartMenu.DAO.Implementation
{
    public class StoreMenuRepository : GenericRepository<StoreMenu>, IStoreMenuRepository
    {
        private readonly SmartMenuDBContext _context;

        public StoreMenuRepository(SmartMenuDBContext context) : base(context)
        {
            _context = context;
        }

        public IEnumerable<StoreMenu> GetAll(int? storeMenuId, int? storeId, int? menuId, string? searchString, int pageNumber = 1, int pageSize = 10)
        {
            var data = _context.StoreMenus.AsQueryable();

            return DataQuery(data, storeId, storeMenuId, menuId, searchString, pageNumber, pageSize);
        }

        private IEnumerable<StoreMenu> DataQuery(IQueryable<StoreMenu> data, int? storeMenuId, int? storeId, int? menuId, string? searchString, int pageNumber, int pageSize)
        {
            data = data.Where(c => c.IsDeleted == false);
            if (storeMenuId != null)
            {
                data = data
                    .Where(c => c.StoreMenuId == storeMenuId);
            }

            if (storeId != null)
            {
                data = data
                    .Where(c => c.StoreId == storeId);
            }

            if (menuId != null)
            {
                data = data
                    .Where(c => c.MenuId == menuId);
            }

            return PaginatedList<StoreMenu>.Create(data, pageNumber, pageSize);
        }
    }
}
