using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SmartMenu.DAO;
using SmartMenu.Domain.Models;
using SmartMenu.Domain.Models.DTO;
using SmartMenu.Domain.Repository;
using SmartMenu.Service.Interfaces;

namespace SmartMenu.Service.Services
{
    public class MenuService : IMenuService
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;

        public MenuService(IMapper mapper, IUnitOfWork unitOfWork)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        public Menu Add(MenuCreateDTO menuCreateDTO)
        {


            var data = _mapper.Map<Menu>(menuCreateDTO);

            _unitOfWork.MenuRepository.Add(data);
            _unitOfWork.Save();

            // Add data for store
            var br = _unitOfWork.BrandRepository
                .Find(c => c.BrandId == menuCreateDTO.BrandId && c.IsDeleted == false)
                .FirstOrDefault()
                ?? throw new Exception("Brand not found or deleted");

            var brandStores = _unitOfWork.BrandRepository.EnableQuery()
                .Include(c => c.Stores)
                .SelectMany(c => c.Stores!)
                .Where(c => c.BrandId == br.BrandId && !c.IsDeleted)
                .ToList();

            foreach (var brandStore in brandStores)
            {
                StoreMenu storeMenu = new()
                {
                    StoreId = brandStore.StoreId,
                    MenuId = data.MenuId,
                };

                _unitOfWork.StoreMenuRepository.Add(storeMenu);
                _unitOfWork.Save();
            }

            return data;
        }

        public void Delete(int menuId)
        {
            var data = _unitOfWork.MenuRepository.Find(c => c.MenuId == menuId && c.IsDeleted == false).FirstOrDefault()
            ?? throw new Exception("Menu not found or deleted");

            data.IsDeleted = true;
            _unitOfWork.MenuRepository.Update(data);
            _unitOfWork.Save();
        }

        public IEnumerable<Menu> GetAll(int? menuId, int? brandId, string? searchString, int pageNumber, int pageSize)
        {
            var data = _unitOfWork.MenuRepository.EnableQuery();
            var result = DataQuery(data, menuId, brandId, searchString, pageNumber, pageSize);

            return result ?? Enumerable.Empty<Menu>();
        }

        public IEnumerable<Menu> GetMenuWithProductGroup(int? menuId, int? brandId, string? searchString, int pageNumber, int pageSize)
        {
            var data = _unitOfWork.MenuRepository.EnableQuery()
                .Include(c => c.ProductGroups!.Where(c => c.IsDeleted == false))
                    .ThenInclude(c => c.ProductGroupItems!.Where(c => !c.IsDeleted))
                        .ThenInclude(c => c.Product!)
                            .ThenInclude(c => c.ProductSizePrices!.Where(c => !c.IsDeleted));

            var result = DataQuery(data, menuId, brandId, searchString, pageNumber, pageSize);

            return result ?? Enumerable.Empty<Menu>();
        }

        public Menu Update(int menuId, MenuUpdateDTO menuUpdateDTO)
        {
            var data = _unitOfWork.MenuRepository.Find(c => c.MenuId == menuId && c.IsDeleted == false).FirstOrDefault()
                ?? throw new Exception("Menu not found or deleted");

            _mapper.Map(menuUpdateDTO, data);
            _unitOfWork.MenuRepository.Update(data);
            _unitOfWork.Save();

            return data;
        }

        private IEnumerable<Menu> DataQuery(IQueryable<Menu> data, int? menuId, int? brandId, string? searchString, int pageNumber, int pageSize)
        {
            data = data.Where(c => c.IsDeleted == false);
            if (menuId != null)
            {
                data = data
                    .Where(c => c.MenuId == menuId);
            }

            if (brandId != null)
            {
                data = data
                    .Where(c => c.BrandId == brandId);
            }

            if (searchString != null)
            {
                searchString = searchString.Trim();
                data = data
                    .Where(c => c.MenuDescription!.Contains(searchString)
                    || c.MenuName.Contains(searchString));
            }

            return PaginatedList<Menu>.Create(data, pageNumber, pageSize);
        }
    }
}