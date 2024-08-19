using SmartMenu.Domain.Models;
using SmartMenu.Domain.Models.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartMenu.Service.Interfaces
{
    public interface IDeviceSubscriptionService
    {
        DeviceSubscription AddDeviceSubscription(DeviceSubscriptionCreateDTO deviceSubscriptionCreateDTO);
        void DeleteDeviceSubscription(int deviceSubscriptionId);
        IEnumerable<DeviceSubscription> GetAll(int? deviceSubscriptionId, int? storeDeviceId, string? searchString, int pageNumber, int pageSize);
        Task<DeviceSubscription> UpdateDeviceSubscription(int deviceSubscriptionId, DeviceSubscriptionUpdateDTO deviceSubscriptionUpdateDTO);
    }
}
