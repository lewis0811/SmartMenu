using SmartMenu.Domain.Models;

namespace SmartMenu.Domain.Repository
{
    public interface IStoreDeviceRepository : IGenericRepository<StoreDevice>
    {
        IEnumerable<StoreDevice> GetAll(int? storeDeviceId, int? storeId, int? displayId, string? searchString, int pageNumber, int pageSize);
    }
}
