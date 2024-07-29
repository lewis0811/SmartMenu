using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SmartMenu.DAO;
using SmartMenu.Domain.Models;
using SmartMenu.Domain.Models.DTO;
using SmartMenu.Domain.Repository;
using SmartMenu.Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartMenu.Service.Services
{
    public class StoreDeviceService : IStoreDeviceService
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;

        public StoreDeviceService(IMapper mapper, IUnitOfWork unitOfWork)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        public StoreDevice Add(StoreDeviceCreateDTO storeDeviceCreateDTO)
        {
            var data = _mapper.Map<StoreDevice>(storeDeviceCreateDTO);
            _unitOfWork.StoreDeviceRepository.Add(data);
            _unitOfWork.Save();
            return data;
        }

        public IEnumerable<StoreDevice> GetAll(int? storeDeviceId, int? storeId, string? searchString, int pageNumber, int pageSize)
        {
            var data = _unitOfWork.StoreDeviceRepository.EnableQuery();
            return DataQuery(data, storeDeviceId, storeId, searchString, pageNumber, pageSize);
        }
        
        public IEnumerable<StoreDevice> GetAllWithDisplays(int? storeDeviceId, int? storeId, string? searchString, int pageNumber, int pageSize)
        {
            var data = _unitOfWork.StoreDeviceRepository.EnableQuery()
                .Include(c => c.Displays);
            return DataQuery(data, storeDeviceId, storeId, searchString, pageNumber, pageSize);
        }
        
        public StoreDevice Update(int storeDeviceId, StoreDeviceUpdateDTO storeDeviceUpdateDTO)
        {
            var data = _unitOfWork.StoreDeviceRepository.Find(c => c.StoreDeviceId == storeDeviceId && c.IsDeleted == false)
                .FirstOrDefault() ?? throw new Exception("StoreDevice not found or deleted");

            _mapper.Map(storeDeviceUpdateDTO, data);
            _unitOfWork.StoreDeviceRepository.Update(data);
            _unitOfWork.Save();

            return data;
        }
        public StoreDevice Update(int storeDeviceId, bool isApproved)
        {
            var data = _unitOfWork.StoreDeviceRepository.Find(c => c.StoreDeviceId == storeDeviceId && c.IsDeleted == false)
                .FirstOrDefault() ?? throw new Exception("StoreDevice not found or deleted");

            data.IsApproved = isApproved;

            _unitOfWork.StoreDeviceRepository.Update(data);
            _unitOfWork.Save();
            return data;
        }
        
        public void Delete(int storeDeviceId)
        {
            var data = _unitOfWork.StoreDeviceRepository.Find(c => c.StoreDeviceId == storeDeviceId && c.IsDeleted == false).FirstOrDefault()
                ?? throw new Exception("StoreDevice not found or deleted");

            data.IsDeleted = true;
            _unitOfWork.StoreDeviceRepository.Update(data);
            _unitOfWork.Save();
        }
        
        private static IEnumerable<StoreDevice> DataQuery(IQueryable<StoreDevice> data, int? storeDeviceId, int? storeId, string? searchString, int pageNumber, int pageSize)
        {
            data = data.Where(c => c.IsDeleted == false);

            if (storeDeviceId != null)
            {
                data = data.Where(c => c.StoreDeviceId == storeDeviceId);
            }

            if (storeId != null)
            {
                data = data.Where(c => c.StoreId == storeId);
            }

            if (searchString != null)
            {
                if (float.TryParse(searchString, out var value))
                {
                    data = data
                        .Where(c => c.DeviceWidth.ToString().Equals(searchString)
                        || c.DeviceHeight.ToString().Equals(searchString));
                }
                data = data
                    .Where(c => c.StoreDeviceName.Equals(searchString)
                    );
            }
            return PaginatedList<StoreDevice>.Create(data, pageNumber, pageSize);
        }

    }
}