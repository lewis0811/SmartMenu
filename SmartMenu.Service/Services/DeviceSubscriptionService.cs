using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SmartMenu.DAO;
using SmartMenu.DAO.Implementation;
using SmartMenu.Domain.Models;
using SmartMenu.Domain.Models.DTO;
using SmartMenu.Domain.Models.Enum;
using SmartMenu.Domain.Repository;
using SmartMenu.Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartMenu.Service.Services
{
    public class DeviceSubscriptionService : IDeviceSubscriptionService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public DeviceSubscriptionService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public IEnumerable<DeviceSubscription> GetAll(int? deviceSubscriptionId, int? storeDeviceId, string? searchString, int pageNumber, int pageSize)
        {
            var data = _unitOfWork.DeviceSubscriptionRepository.EnableQuery()
                .Include(c => c.StoreDevice).Where(c => !c.IsDeleted)
                .Include(c => c.Subscription).Where(c => !c.IsDeleted)
                .Include(c => c.Transactions!.Where(d => !d.IsDeleted));

            var result = DataQuery(data, deviceSubscriptionId, storeDeviceId, searchString, pageNumber, pageSize);
            return result;
        }

        public async Task<DeviceSubscription> AddDeviceSubscription(DeviceSubscriptionCreateDTO deviceSubscriptionCreateDTO)
        {
            var existDeviceSubscription =  _unitOfWork.DeviceSubscriptionRepository
                .Find(c => c.StoreDeviceId == deviceSubscriptionCreateDTO.StoreDeviceId && c.IsDeleted == false).FirstOrDefault();

            if (existDeviceSubscription != null)
            {
                throw new Exception("DeviceSubscription already exists.");
            }

            var storeDevice =  _unitOfWork.StoreDeviceRepository.Find(c => c.StoreDeviceId == deviceSubscriptionCreateDTO.StoreDeviceId && c.IsDeleted == false).FirstOrDefault()
                ?? throw new Exception("Store device not found");

            var data = _mapper.Map<DeviceSubscription>(deviceSubscriptionCreateDTO);

            _unitOfWork.DeviceSubscriptionRepository.Add(data);
            _unitOfWork.Save();

            return data;
        }

        public async Task<DeviceSubscription> UpdateDeviceSubscription(int deviceSubscriptionId, DeviceSubscriptionUpdateDTO deviceSubscriptionUpdateDTO)
        {
            var existDeviceSubscription = await _unitOfWork.DeviceSubscriptionRepository.FindObjectAsync(c => c.DeviceSubscriptionId == deviceSubscriptionId && c.IsDeleted == false)
                ?? throw new Exception("DeviceSubscription not found or deleted.");

            _mapper.Map(deviceSubscriptionUpdateDTO, existDeviceSubscription);

            _unitOfWork.DeviceSubscriptionRepository.Update(existDeviceSubscription);
            _unitOfWork.Save();

            return existDeviceSubscription;
        }

        public void DeleteDeviceSubscription(int deviceSubscriptionId)
        {
            var existDeviceSubscription =  _unitOfWork.DeviceSubscriptionRepository.Find(c => c.DeviceSubscriptionId == deviceSubscriptionId && c.IsDeleted == false).FirstOrDefault()
                ?? throw new Exception("DeviceSubscription not found or deleted.");

            existDeviceSubscription.IsDeleted = true;
            _unitOfWork.DeviceSubscriptionRepository.Update(existDeviceSubscription);
            _unitOfWork.Save();
        }

        private static IEnumerable<DeviceSubscription> DataQuery(IQueryable<DeviceSubscription> data, int? deviceSubscriptionId, int? storeDeviceId, string? searchString, int pageNumber, int pageSize)
        {
            data = data.Where(c => c.IsDeleted == false);

            if (deviceSubscriptionId != null)
            {
                data = data.Where(c => c.DeviceSubscriptionId == deviceSubscriptionId);
            }

            if (storeDeviceId != null)
            {
                data = data.Where(c => c.StoreDeviceId == storeDeviceId);
            }

            if (searchString != null)
            {
                if (Enum.TryParse(typeof(SubscriptionStatus), searchString, out var subscriptionStatus))
                {
                    data = data.Where(c => c.SubscriptionStatus == (SubscriptionStatus)subscriptionStatus!);
                }
            }

            return PaginatedList<DeviceSubscription>.Create(data, pageNumber, pageSize);
        }
    }
}