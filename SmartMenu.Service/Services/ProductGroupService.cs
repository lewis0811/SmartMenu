using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SmartMenu.DAO;
using SmartMenu.Domain.Models;
using SmartMenu.Domain.Models.DTO;
using SmartMenu.Domain.Repository;
using SmartMenu.Service.Interfaces;

namespace SmartMenu.Service.Services
{
    public class ProductGroupService : IProductGroupService
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;

        public ProductGroupService(IMapper mapper, IUnitOfWork unitOfWork)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        public ProductGroup Add(ProductGroupCreateDTO productGroupCreateDTO)
        {
            if (productGroupCreateDTO.CollectionID == 0) productGroupCreateDTO.CollectionID = null;
            if (productGroupCreateDTO.MenuID == 0) productGroupCreateDTO.MenuID = null;
            if (productGroupCreateDTO.CollectionID != null && productGroupCreateDTO.MenuID != null)
            {
                throw new Exception("Product group can't be in both menu, collection");
            }

            switch(productGroupCreateDTO.MenuID != null)
            {
                case true:
                    var existProductGroup = _unitOfWork.ProductGroupRepository.Find(c => c.MenuId == productGroupCreateDTO.MenuID && c.ProductGroupName == productGroupCreateDTO.ProductGroupName && !c.IsDeleted).FirstOrDefault();
                    if (existProductGroup != null) throw new Exception($"Product group name: {productGroupCreateDTO.ProductGroupName} already exist in menu Id: {existProductGroup.MenuId}");
                    break;
                case false:
                    var existProductGroup2 = _unitOfWork.ProductGroupRepository.Find(c => c.CollectionId == productGroupCreateDTO.CollectionID && c.ProductGroupName == productGroupCreateDTO.ProductGroupName && !c.IsDeleted).FirstOrDefault();
                    if (existProductGroup2 != null) throw new Exception($"Product group name: {productGroupCreateDTO.ProductGroupName} already exist in collection Id: {existProductGroup2.CollectionId}");
                    break;
            }


            var data = _mapper.Map<ProductGroup>(productGroupCreateDTO);

            _unitOfWork.ProductGroupRepository.Add(data);
            _unitOfWork.Save();

            return data;
        }

        public void Delete(int productGroupId)
        {
            var data = _unitOfWork.ProductGroupRepository.Find(c => c.ProductGroupId == productGroupId && c.IsDeleted == false).FirstOrDefault()
            ?? throw new Exception("Product group not found or deleted");

            data.IsDeleted = true;
            _unitOfWork.ProductGroupRepository.Update(data);
            _unitOfWork.Save();
        }

        public IEnumerable<ProductGroup> GetAll(int? productGroupId, int? menuId, int? collectionId, string? searchString, int pageNumber, int pageSize)
        {
            var data = _unitOfWork.ProductGroupRepository.EnableQuery();
            var result = DataQuery(data, productGroupId, menuId, collectionId, searchString, pageNumber, pageSize);

            return result ?? Enumerable.Empty<ProductGroup>();
        }

        public IEnumerable<ProductGroup> GetProductGroupWithGroupItem(int? productGroupId, int? menuId, int? collectionId, string? searchString, int pageNumber, int pageSize)
        {
            var data = _unitOfWork.ProductGroupRepository.EnableQuery();
            data = data.Include(c => c.ProductGroupItems!.Where(c => c.IsDeleted == false))
                .ThenInclude(c => c.Product!)
                    .ThenInclude(c => c.ProductSizePrices!.Where(d => d.IsDeleted == false));

            var result = DataQuery(data, productGroupId, menuId, collectionId, searchString, pageNumber, pageSize);

            return result ?? Enumerable.Empty<ProductGroup>();
        }

        public ProductGroup Update(int productGroupId, ProductGroupUpdateDTO productGroupUpdateDTO)
        {
            var data = _unitOfWork.ProductGroupRepository.Find(c => c.ProductGroupId == productGroupId && c.IsDeleted == false).FirstOrDefault()
                ?? throw new Exception("Product group not found or deleted");

            if(productGroupUpdateDTO.HaveNormalPrice != data.HaveNormalPrice)
            {
                var productGroupItems = _unitOfWork.ProductGroupItemRepository
                    .Find(c => c.ProductGroupId == productGroupId)
                    .ToList();
                _unitOfWork.ProductGroupItemRepository.RemoveRange(productGroupItems);
                _unitOfWork.Save();
            }

            _mapper.Map(productGroupUpdateDTO, data);
            _unitOfWork.ProductGroupRepository.Update(data);
            _unitOfWork.Save();

            return data;
        }

        private IEnumerable<ProductGroup> DataQuery(IQueryable<ProductGroup> data, int? productGroupId, int? menuId, int? collectionId, string? searchString, int pageNumber, int pageSize)
        {
            data = data.Where(c => c.IsDeleted == false);
            if (productGroupId != null)
            {
                data = data
                    .Where(c => c.ProductGroupId == productGroupId);
            }

            if (menuId != null)
            {
                data = data
                    .Where(c => c.MenuId == menuId);
            }
            if (collectionId != null)
            {
                data = data
                    .Where(c => c.CollectionId == collectionId);
            }

            if (searchString != null)
            {
                searchString = searchString.Trim();
                data = data
                    .Where(c => c.ProductGroupName.Contains(searchString));
            }

            return PaginatedList<ProductGroup>.Create(data, pageNumber, pageSize);
        }
    }
}