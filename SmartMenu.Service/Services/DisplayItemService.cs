using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SmartMenu.DAO;
using SmartMenu.Domain.Models;
using SmartMenu.Domain.Models.DTO;
using SmartMenu.Domain.Repository;
using SmartMenu.Service.Interfaces;

namespace SmartMenu.Service.Services
{
    public class DisplayItemService : IDisplayItemService
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;

        public DisplayItemService(IMapper mapper, IUnitOfWork unitOfWork)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        public IEnumerable<DisplayItem> GetAll(int? displayItemId, int? displayId, int? boxId, int? productGroupId, string? searchString, int pageNumber, int pageSize)
        {
            var data = _unitOfWork.DisplayItemRepository.EnableQuery()
                .Include(c => c.Box).Where(d => d.Box!.IsDeleted == false)
                .Include(c => c.ProductGroup).Where(d => d.ProductGroup!.IsDeleted == false);
            var result = DataQuery(data, displayItemId, displayId, boxId, productGroupId, searchString, pageNumber, pageSize);

            return result;
        }

        public DisplayItem AddDisplayItem(DisplayItemCreateDTO displayItemCreateDTO)
        {
            var data = _mapper.Map<DisplayItem>(displayItemCreateDTO);
            _unitOfWork.DisplayItemRepository.Add(data);
            _unitOfWork.Save();

            UpdateDisplayIfExist(data);

            return data;
        }

        public DisplayItem Update(int displayItemId, DisplayItemUpdateDTO displayItemUpdateDTO)
        {
            var displayItem = _unitOfWork.DisplayItemRepository.Find(c => c.DisplayItemId == displayItemId && !c.IsDeleted)
                .FirstOrDefault() ?? throw new Exception("Display Item not found or deleted");

            var display = _unitOfWork.DisplayRepository.Find(c => c.DisplayId == displayItem.DisplayId && !c.IsDeleted).FirstOrDefault()!;

            switch (display.MenuId)
            {
                case null:
                    var isExistCollection = _unitOfWork.StoreCollectionRepository.EnableQuery()
                        .Include(c => c.Collection!)
                            .ThenInclude(c => c.ProductGroups)
                        .Where(c => !c.IsDeleted && !c.Collection!.IsDeleted)
                            .SelectMany(c => c.Collection!.ProductGroups!)
                        .FirstOrDefault(c => c.ProductGroupId == displayItemUpdateDTO.ProductGroupID && !c.IsDeleted)
                        ?? throw new Exception($"Product group not exist in collection Id: {display.CollectionId}");
                    break;

                default:
                    var isExistMenu = _unitOfWork.StoreMenuRepository.EnableQuery()
                        .Include(c => c.Menu!)
                            .ThenInclude(c => c.ProductGroups)
                        .Where(c => !c.IsDeleted && !c.Menu!.IsDeleted)
                            .SelectMany(c => c.Menu!.ProductGroups!)
                        .FirstOrDefault(c => c.ProductGroupId == displayItemUpdateDTO.ProductGroupID && !c.IsDeleted)
                        ?? throw new Exception($"Product group not exist in menu Id: {display.MenuId}");
                    break;
            }

            var tempProductGroupId = displayItem.ProductGroupId;
            _mapper.Map(displayItemUpdateDTO, displayItem);

            var matchedProductgroup = _unitOfWork.DisplayItemRepository.Find(c => c.ProductGroupId == displayItemUpdateDTO.ProductGroupID && c.DisplayId == displayItem.DisplayId && !c.IsDeleted)
                .FirstOrDefault();
            if (matchedProductgroup != null)
            {
                matchedProductgroup.ProductGroupId = tempProductGroupId;
                _unitOfWork.DisplayItemRepository.Update(matchedProductgroup);
                _unitOfWork.Save();
            }

            _unitOfWork.DisplayItemRepository.Update(displayItem);
            _unitOfWork.Save();

            UpdateDisplayIfExist(displayItem);

            return displayItem;
        }

        public void Delete(int displayItemId)
        {
            var data = _unitOfWork.DisplayItemRepository.Find(c => c.DisplayItemId == displayItemId).FirstOrDefault()
            ?? throw new Exception("DisplayItem not found or deleted");

            _unitOfWork.DisplayItemRepository.Remove(data);
            _unitOfWork.Save();

            UpdateDisplayIfExist(data);
        }

        private static IEnumerable<DisplayItem> DataQuery(IQueryable<DisplayItem> data, int? displayItemId, int? displayId, int? boxId, int? productGroupId, string? searchString, int pageNumber, int pageSize)
        {
            data = data.Where(c => c.IsDeleted == false);

            if (displayItemId != null)
            {
                data = data.Where(c => c.DisplayItemId == displayItemId);
            }

            if (displayId != null)
            {
                data = data.Where(c => c.DisplayId == displayId);
            }

            if (boxId != null)
            {
                data = data.Where(c => c.BoxId == boxId);
            }

            if (productGroupId != null)
            {
                data = data.Where(c => c.ProductGroupId == productGroupId);
            }

            if (searchString != null)
            {
                data = data.Where(c => c.Box!.BoxWidth.ToString().Contains(searchString)
                || c.Box.BoxHeight.ToString().Contains(searchString)
                || c.Box.BoxPositionX.ToString().Contains(searchString)
                || c.Box.BoxPositionY.ToString().Contains(searchString)
                || c.ProductGroup!.ProductGroupName.Contains(searchString));
            }

            return PaginatedList<DisplayItem>.Create(data, pageNumber, pageSize);
        }

        private void UpdateDisplayIfExist(DisplayItem data)
        {
            var display = _unitOfWork.DisplayRepository.Find(c => c.DisplayId == data.DisplayId && !c.IsDeleted).FirstOrDefault();
            if (display != null)
            {
                display.IsChanged = true;
                _unitOfWork.DisplayRepository.Update(display);
                _unitOfWork.Save();
            }
        }
    }
}