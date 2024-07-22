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
    public class StoreMenuService : IStoreMenuService
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;

        public StoreMenuService(IMapper mapper, IUnitOfWork unitOfWork)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }
        public StoreMenu Add(StoreMenuCreateDTO storeMenuCreateDTO)
        {
            var st = _unitOfWork.StoreRepository
                .Find(c => c.StoreId == storeMenuCreateDTO.StoreID && c.IsDeleted == false)
                .FirstOrDefault()
                ?? throw new Exception("Store not found or deleted");

            var mn = _unitOfWork.MenuRepository
                .Find(c => c.MenuId == storeMenuCreateDTO.MenuID && c.IsDeleted == false)
                .FirstOrDefault()
                ?? throw new Exception("Store not found or deleted");

            var data = _mapper.Map<StoreMenu>(storeMenuCreateDTO);

            _unitOfWork.StoreMenuRepository.Add(data);
            _unitOfWork.Save();

            return data;
        }

        public void Delete(int storeMenuId)
        {
            var data = _unitOfWork.StoreMenuRepository.Find(c => c.StoreMenuId == storeMenuId && c.IsDeleted == false).FirstOrDefault()
            ?? throw new Exception("Store Menu not found or deleted");

            data.IsDeleted = true;
            _unitOfWork.StoreMenuRepository.Update(data);
            _unitOfWork.Save();
        }

        public IEnumerable<StoreMenu> GetAll(int? storeMenuId, int? storeId, int? menuId, string? searchString, int pageNumber, int pageSize)
        {
            var data = _unitOfWork.StoreMenuRepository.EnableQuery()
                .Include(c => c.Menu)
                .Where(c => !c.Menu!.IsDeleted);

            var result = DataQuery(data, storeMenuId, storeId, menuId, searchString, pageNumber, pageSize);

            return result ?? Enumerable.Empty<StoreMenu>();
        }

        public StoreMenu Update(int storeMenuId, StoreMenuCreateDTO storeMenuCreateDTO)
        {
            var data = _unitOfWork.StoreMenuRepository.Find(c => c.StoreMenuId == storeMenuId && c.IsDeleted == false).FirstOrDefault()
                ?? throw new Exception("Store Menu not found or deleted");

            _mapper.Map(storeMenuCreateDTO, data);
            _unitOfWork.StoreMenuRepository.Update(data);
            _unitOfWork.Save();

            return data;
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
