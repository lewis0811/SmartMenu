using SmartMenu.Domain.Models;
using SmartMenu.Domain.Repository;

namespace SmartMenu.DAO.Implementation
{
    public class StoreDeviceRepository : GenericRepository<StoreDevice>, IStoreDeviceRepository
    {
        private readonly SmartMenuDBContext _context;

        public StoreDeviceRepository(SmartMenuDBContext context) : base(context)
        {
            _context = context;
        }

        public IEnumerable<StoreDevice> GetAll(int? storeDeviceId, int? storeId, int? displayId, string? searchString, int pageNumber, int pageSize)
        {
            var data = _context.StoreDevices.AsQueryable();
            return DataQuery(data, storeDeviceId, storeId, displayId, searchString, pageNumber, pageSize);
        }

        private IEnumerable<StoreDevice> DataQuery(IQueryable<StoreDevice> data, int? storeDeviceId, int? storeId, int? displayId, string? searchString, int pageNumber, int pageSize)
        {
            data = data.Where(c => c.IsDeleted == false);
            if (storeDeviceId != null)
            {
                data = data
                    .Where(c => c.StoreDeviceID == storeDeviceId);
            }

            if (storeId != null)
            {
                data = data
                    .Where(c => c.StoreID == storeId);
            }

            if (displayId != null)
            {
                data = data
                    .Where(c => c.DisplayID == displayId);
            }

            if (searchString != null)
            {
                searchString = searchString.Trim();
                data = data
                    .Where(c => c.StoreDeviceName.Contains(searchString)
                    || c.DisplayType.ToString().Contains(searchString));
            }

            return data;
        }
    }
}