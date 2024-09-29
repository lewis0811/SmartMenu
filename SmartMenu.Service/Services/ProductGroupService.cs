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

            switch (productGroupCreateDTO.MenuID != null)
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

            UpdateDisplayIfExist(data.MenuId, data.CollectionId, data.ProductGroupId);

            return data;
        }

        public void Delete(int productGroupId)
        {
            var data = _unitOfWork.ProductGroupRepository.Find(c => c.ProductGroupId == productGroupId && c.IsDeleted == false).FirstOrDefault()
            ?? throw new Exception("Product group not found or deleted");

            //data.IsDeleted = true;
            _unitOfWork.ProductGroupRepository.Remove(data);
            _unitOfWork.Save();

            UpdateDisplayIfExist(data.MenuId, data.CollectionId, data.ProductGroupId);
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

            if (productGroupUpdateDTO.HaveNormalPrice != data.HaveNormalPrice)
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

            UpdateDisplayIfExist(data.MenuId, data.CollectionId, data.ProductGroupId);

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

        private void UpdateDisplayIfExist(int? menuId, int? collectionId, int productGroupId)
        {
            List<Display> displays = new();
            List<ProductGroup> existProductGroup = new();
            if (menuId != null)
            {
                displays = _unitOfWork.DisplayRepository
                    .EnableQuery()
                    .Where(c => c.CollectionId == collectionId && !c.IsDeleted).ToList();

                existProductGroup = _unitOfWork.ProductGroupRepository
                    .EnableQuery()
                    .Where(c => c.CollectionId == collectionId && !c.IsDeleted).ToList();
            }

            if (collectionId != null)
            {
                displays = _unitOfWork.DisplayRepository
                    .EnableQuery()
                    .Where(c => c.MenuId == menuId && !c.IsDeleted).ToList();

                existProductGroup = _unitOfWork.ProductGroupRepository
                    .EnableQuery()
                    .Where(c => c.MenuId == menuId && !c.IsDeleted).ToList();
            }

            if (displays.Count > 0)
            {
                foreach (var display in displays)
                {
                    display.IsChanged = true;
                    _unitOfWork.DisplayRepository.Update(display);
                    _unitOfWork.Save();

                    var existDisplayItemBoxes = _unitOfWork.DisplayItemRepository
                        .EnableQuery()
                        .Where(c => c.DisplayId == display.DisplayId)

                        .Select(c => c.Box!)
                        .ToList();

                    // Case adding product group, adding it to any display have it menu / collection too
                    if (existDisplayItemBoxes.Count > 0 && existDisplayItemBoxes.Count < existProductGroup.Count)
                    {
                        var renderBox = _unitOfWork.DisplayRepository.EnableQuery()
                            .Where(c => c.DisplayId == display.DisplayId && !c.IsDeleted)
                            .Include(c => c.Template!)
                                .ThenInclude(c => c.Layers!.Where(c => !c.IsDeleted && c.LayerType == Domain.Models.Enum.LayerType.Content))
                                    .ThenInclude(c => c.Boxes!.Where(c => !c.IsDeleted && c.BoxType == Domain.Models.Enum.BoxType.UseInDisplay))
                            .Select(c => c.Template!)
                                .SelectMany(c => c.Layers!.Where(c => c.LayerType == Domain.Models.Enum.LayerType.Content && !c.IsDeleted))
                                    .SelectMany(c => c.Boxes!.Where(c => c.BoxType == Domain.Models.Enum.BoxType.UseInDisplay && !c.IsDeleted))
                            .ToList();

                        if (renderBox.Count > 0)
                        {
                            var neededBox = renderBox.Except(existDisplayItemBoxes).FirstOrDefault();

                            if (neededBox != null)
                            {
                                DisplayItem displayItem = new()
                                {
                                    BoxId = neededBox.BoxId,
                                    DisplayId = display.DisplayId,
                                    ProductGroupId = productGroupId
                                };
                                _unitOfWork.DisplayItemRepository.Add(displayItem);
                                _unitOfWork.Save();
                            }
                        }
                    }

                    // Case delete product group, remove it from any display have it menu / collection too
                    if (existDisplayItemBoxes.Count > 0 && existDisplayItemBoxes.Count > existProductGroup.Count)
                    {
                        var existProductGroupInDisplayItem = _unitOfWork.DisplayItemRepository
                            .EnableQuery()
                            .Include(c => c.ProductGroup)
                            .Where(c => c.DisplayId == display.DisplayId && !c.IsDeleted)
                            .Select(c => c.ProductGroup!)
                            .ToList();

                        if (existProductGroupInDisplayItem.Count > 0)
                        {
                            var productGroupNeedToDelete = existProductGroupInDisplayItem.Except(existProductGroup).ToList();

                            if (productGroupNeedToDelete.Count > 0)
                            {
                                foreach (var productGroup in productGroupNeedToDelete)
                                {
                                    var matchedDisplayItem = _unitOfWork.DisplayItemRepository.EnableQuery()
                                        .FirstOrDefault(c => c.ProductGroupId == productGroup.ProductGroupId && !c.IsDeleted)!;
                                    _unitOfWork.DisplayItemRepository.Remove(matchedDisplayItem);
                                    _unitOfWork.Save();
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}