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
                ?? throw new Exception("Menu not found or deleted");

            var data = _mapper.Map<StoreMenu>(storeMenuCreateDTO);

            _unitOfWork.StoreMenuRepository.Add(data);
            _unitOfWork.Save();

            return data;
        }

        public void Delete(int storeMenuId)
        {
            var data = _unitOfWork.StoreMenuRepository.Find(c => c.StoreMenuId == storeMenuId).FirstOrDefault()
                ?? throw new Exception("Store Menu not found or deleted");

            _unitOfWork.StoreMenuRepository.Remove(data);
            _unitOfWork.Save();
        }

        public void DeleteV2(int storeMenuId)
        {
            var data = _unitOfWork.StoreMenuRepository.Find(c => c.StoreMenuId == storeMenuId).FirstOrDefault()
                ?? throw new Exception("Store Menu not found or deleted");

            data.IsDeleted = !data.IsDeleted;
            _unitOfWork.StoreMenuRepository.Update(data);
            _unitOfWork.Save();
        }

        public IEnumerable<StoreMenu> GetAll(int? storeMenuId, int? storeId, int? menuId, string? searchString, int pageNumber, int pageSize)
        {
            var data = _unitOfWork.StoreMenuRepository.EnableQuery()
                .Include(c => c.Menu)
                    .ThenInclude(c => c.ProductGroups!.Where(c => !c.IsDeleted))
                        .ThenInclude(c => c.ProductGroupItems!.Where(c => !c.IsDeleted))
                            .ThenInclude(c => c.Product!)
                                .ThenInclude(c => c.ProductSizePrices!.Where(c => !c.IsDeleted))
                .Where(c => !c.Menu!.IsDeleted);

            if (storeId != null)
            {
                AddMenuForStore(storeId);

            }

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

        private void AddMenuForStore(int? storeId)
        {
            var store = _unitOfWork.StoreRepository.EnableQuery()
                .Include(c => c.StoreMenus.Where(c => !c.IsDeleted))
                .FirstOrDefault(c => c.StoreId == storeId && !c.IsDeleted)
                ?? throw new Exception("Store not found or deleted");

            var menus = _unitOfWork.MenuRepository.EnableQuery()
                .Where(c => c.BrandId == store.BrandId && !c.IsDeleted)
                .ToList();

            if (store.StoreMenus.Count < menus.Count)
            {
                foreach (var menu in menus)
                {
                    if (store.StoreMenus != null && store.StoreMenus.Any(c => c.MenuId == menu.MenuId)) continue;
                    {
                        StoreMenu storeMenu = new()
                        {
                            MenuId = menu.MenuId,
                            StoreId = store.StoreId,
                        };
                        _unitOfWork.StoreMenuRepository.Add(storeMenu);
                        _unitOfWork.Save();
                    }
                }
            }
        }

        private IEnumerable<StoreMenu> DataQuery(IQueryable<StoreMenu> data, int? storeMenuId, int? storeId, int? menuId, string? searchString, int pageNumber, int pageSize)
        {
            //data = data.Where(c => c.IsDeleted == false);
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

            if (searchString != null)
            {
                searchString = searchString.Trim();
                data = data
                    .Where(c => c.Menu!.MenuName.Contains(searchString)
                    || c.Menu!.MenuDescription!.Contains(searchString));
            }

            return PaginatedList<StoreMenu>.Create(data, pageNumber, pageSize);
        }
    }
}