using SmartMenu.Domain.Models;
using SmartMenu.Domain.Models.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartMenu.Service.Interfaces
{
    public interface IStoreDeviceService
    {
        IEnumerable<StoreDevice> GetAll(int? storeDeviceId, int? storeId, string? searchString, int pageNumber, int pageSize);
        StoreDevice Add(StoreDeviceCreateDTO storeDeviceCreateDTO);
        IEnumerable<StoreDevice> GetAllWithDisplays(int? storeDeviceId, int? storeId, string? searchString, int pageNumber, int pageSize);
        StoreDevice Update(int storeDeviceId, StoreDeviceUpdateDTO storeDeviceUpdateDTO);
        void Delete(int storeDeviceId);
        StoreDevice Update(int storeDeviceId, bool isApproved);
    }
}
