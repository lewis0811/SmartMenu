using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SmartMenu.DAO;
using SmartMenu.Domain.Models;
using SmartMenu.Domain.Models.DTO;
using SmartMenu.Domain.Models.Enum;
using SmartMenu.Domain.Repository;
using SmartMenu.Service.Interfaces;

namespace SmartMenu.Service.Services
{
    public class BoxItemService : IBoxItemService
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;

        public BoxItemService(IMapper mapper, IUnitOfWork unitOfWork)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        public BoxItem AddBoxItem(BoxItemCreateDTO boxItemCreateDTO)
        {
            var box = _unitOfWork.BoxRepository.Find(c => c.BoxId == boxItemCreateDTO.BoxId && c.IsDeleted == false).FirstOrDefault()
            ?? throw new Exception("Box not found or deleted");

            var font = _unitOfWork.FontRepository.Find(c => c.FontId == boxItemCreateDTO.FontId && c.IsDeleted == false).FirstOrDefault()
            ?? throw new Exception("Font not found or deleted");

            var layer = _unitOfWork.LayerRepository.Find(c => c.LayerId == box.LayerId && c.IsDeleted == false).FirstOrDefault()
            ?? throw new Exception("Layer not found or deleted");

            var boxItem = _unitOfWork.BoxItemRepository.EnableQuery();

            //Validate boxItem to add
            if (layer.LayerType == LayerType.BackgroundImageLayer) throw new Exception("Background image can't have box item");

            if (layer.LayerType == LayerType.ImageLayer) throw new Exception("Image can't have box item");

            if (layer.LayerType == LayerType.RenderLayer)
            {
                boxItem = boxItem.Where(c => c.BoxId == box.BoxId && c.IsDeleted == false);

                if (boxItem.Count() > 1) // there is already 2 box item in render layer
                {
                    throw new Exception("Render layer can't have more than 2 box item");
                }

                if (boxItemCreateDTO.BoxItemType == BoxItemType.Header)
                {
                    var boxItemHeader = _unitOfWork.BoxItemRepository.Find(c => c.BoxId == box.BoxId && c.BoxItemType == BoxItemType.Header)
                        .FirstOrDefault();
                    if (boxItemHeader != null)  throw new Exception($"Box item type header already exist in box ID: {box.BoxId}"); 
                }

                if (boxItemCreateDTO.BoxItemType == BoxItemType.Body)
                {
                    var boxItemBody = _unitOfWork.BoxItemRepository.Find(c => c.BoxId == box.BoxId && c.BoxItemType == BoxItemType.Body)
                        .FirstOrDefault();
                    if (boxItemBody != null) throw new Exception($"Box item type body already exist in box ID: {box.BoxId}"); 
                }
            }

            var data = _mapper.Map<BoxItem>(boxItemCreateDTO);
                _unitOfWork.BoxItemRepository.Add(data);
                _unitOfWork.Save();

            return data;
        }

        public void Delete(int boxItemId)
        {
            var data = _unitOfWork.BoxItemRepository.Find(c => c.BoxItemId == boxItemId && c.IsDeleted == false).FirstOrDefault()
            ?? throw new Exception("BoxItem not found or deleted");

            data.IsDeleted = true;
            _unitOfWork.BoxItemRepository.Update(data);
            _unitOfWork.Save();
        }

        public IEnumerable<BoxItem> GetAll(int? boxItemId, int? boxId, int? fontId, string? searchString, int pageNumber, int pageSize)
        {
            var data = _unitOfWork.BoxItemRepository.EnableQuery()
                .Include(c => c.Font);
            var result = DataQuery(data, boxItemId, boxId, fontId, searchString, pageNumber, pageSize);

            return result;
        }

        public BoxItem Update(int boxItemId, BoxItemUpdateDTO boxItemUpdateDTO)
        {
            var data = _unitOfWork.BoxItemRepository.Find(c => c.BoxItemId == boxItemId && c.IsDeleted == false).FirstOrDefault()
            ?? throw new Exception("BoxItem not found or deleted");

            var font = _unitOfWork.FontRepository.Find(c => c.FontId == boxItemUpdateDTO.FontId && c.IsDeleted == false).FirstOrDefault()
            ?? throw new Exception("Font not found or deleted");

            _mapper.Map(boxItemUpdateDTO, data);

            _unitOfWork.BoxItemRepository.Update(data);
            _unitOfWork.Save();

            return data;
        }

        private IEnumerable<BoxItem> DataQuery(IQueryable<BoxItem> data, int? boxItemId, int? boxId, int? fontId, string? searchString, int pageNumber, int pageSize)
        {
            data = data.Where(c => c.IsDeleted == false);

            if (boxItemId != null)
            {
                data = data.Where(c => c.BoxItemId == boxItemId);
            }

            if (boxId != null)
            {
                data = data.Where(c => c.BoxId == boxId);
            }

            if (fontId != null)
            {
                data = data.Where(c => c.FontId == fontId);
            }

            if (searchString != null)
            {
                data = data.Where(c => c.TextFormat.ToString() == searchString
                || c.FontSize.ToString() == searchString
                || c.BoxColor == searchString
                || c.BoxItemType.ToString() == searchString
                );
            }

            return PaginatedList<BoxItem>.Create(data, pageNumber, pageSize);
        }
    }
}