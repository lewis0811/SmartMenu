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
                .Include(c => c.Box)
                .Include(c => c.ProductGroup);
            var result = DataQuery(data, displayItemId, displayId, boxId, productGroupId, searchString, pageNumber, pageSize);

            return result;
        }

        public DisplayItem AddDisplayItem(DisplayItemCreateDTO displayItemCreateDTO)
        {
            var data = _mapper.Map<DisplayItem>(displayItemCreateDTO);
            _unitOfWork.DisplayItemRepository.Add(data);
            _unitOfWork.Save();
            return data;
        }

        public DisplayItem Update(int displayItemId, DisplayItemUpdateDTO displayItemUpdateDTO)
        {
            var data = _mapper.Map<DisplayItem>(displayItemUpdateDTO);
            _unitOfWork.DisplayItemRepository.Update(data);
            _unitOfWork.Save();

            return data;
        }

        public void Delete(int displayItemId)
        {
            var data = _unitOfWork.DisplayItemRepository.Find(c => c.DisplayItemId == displayItemId).FirstOrDefault()
            ?? throw new Exception("DisplayItem not found or deleted");

            _unitOfWork.DisplayItemRepository.Remove(data);
            _unitOfWork.Save();
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
                data = data.Where(c => c.Box!.BoxMaxCapacity.ToString().Contains(searchString)
                || c.Box.BoxWidth.ToString().Contains(searchString)
                || c.Box.BoxHeight.ToString().Contains(searchString)
                || c.Box.BoxPositionX.ToString().Contains(searchString)
                || c.Box.BoxPositionY.ToString().Contains(searchString)
                || c.ProductGroup!.ProductGroupName.Contains(searchString));
            }

            return PaginatedList<DisplayItem>.Create(data, pageNumber, pageSize);
        }
    }
}