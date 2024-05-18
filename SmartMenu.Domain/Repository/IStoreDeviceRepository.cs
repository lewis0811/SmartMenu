using SmartMenu.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartMenu.Domain.Repository
{
    public interface IStoreDeviceRepository : IGenericRepository<StoreDevice>
    {
        IEnumerable<StoreDevice> GetAll(int? storeDeviceId, int? storeId, int? displayId, string? searchString, int pageNumber, int pageSize);
    }
}
