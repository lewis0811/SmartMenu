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

            var font = _unitOfWork.FontRepository.Find(c => c.BFontId == boxItemCreateDTO.BFontId && c.IsDeleted == false).FirstOrDefault()
                ?? throw new Exception("Font not found or deleted");

            var layer = _unitOfWork.LayerRepository.Find(c => c.LayerId == box.LayerId && c.IsDeleted == false).FirstOrDefault()
            ?? throw new Exception("Layer not found or deleted");

            var data = _mapper.Map<BoxItem>(boxItemCreateDTO);

            //Validate boxItem to add
            if (layer.LayerType == LayerType.BackgroundImage) throw new Exception("Background image can't have box item");

            if (box.BoxType == BoxType.UseInTemplate)
            {
                var existBoxItem = _unitOfWork.BoxItemRepository.EnableQuery()!.Any(c => c.BoxId == data.BoxId);
                if (existBoxItem) throw new Exception($"BoxItem already exists in box id {box.BoxId}");
            }

            switch (data.BoxItemType)
            {
                case BoxItemType.Header:
                    var existBoxItem = _unitOfWork.BoxItemRepository.EnableQuery()!.Any(c => c.BoxId == data.BoxId && c.BoxItemType == BoxItemType.Header);
                    if (existBoxItem) throw new Exception($"BoxItem type header already exists in box id {box.BoxId}");
                    data.Order = 1;
                    break;

                case BoxItemType.ProductName:
                    var totalProductName = _unitOfWork.BoxItemRepository.EnableQuery()!.Where(c => c.BoxId == data.BoxId && c.BoxItemType == BoxItemType.ProductName).Count();
                    if (totalProductName >= box.MaxProductItem) throw new Exception($"BoxItem type product name already reach max product item :{box.MaxProductItem}");
                    data.Order = totalProductName + 1;
                    break;

                case BoxItemType.ProductDescription:
                    var totalProductDescription = _unitOfWork.BoxItemRepository.EnableQuery()!.Where(c => c.BoxId == data.BoxId && c.BoxItemType == BoxItemType.ProductDescription).Count();
                    if (totalProductDescription >= box.MaxProductItem) throw new Exception($"BoxItem type product description already reach max product item :{box.MaxProductItem}");
                    data.Order = totalProductDescription + 1;
                    break;

                case BoxItemType.ProductPrice:
                    var totalProductPrice = _unitOfWork.BoxItemRepository.EnableQuery()!.Where(c => c.BoxId == data.BoxId && c.BoxItemType == BoxItemType.ProductPrice).Count();
                    if (totalProductPrice >= box.MaxProductItem) throw new Exception($"BoxItem type product price already reach max product item :{box.MaxProductItem}");
                    data.Order = totalProductPrice + 1;
                    break;

                case BoxItemType.ProductImg:
                    var totalProductImage = _unitOfWork.BoxItemRepository.EnableQuery()!.Where(c => c.BoxId == data.BoxId && c.BoxItemType == BoxItemType.ProductImg).Count();
                    if (totalProductImage >= box.MaxProductItem) throw new Exception($"BoxItem type product image already reach max product item :{box.MaxProductItem}");
                    data.Order = totalProductImage + 1;
                    break;

                case BoxItemType.ProductIcon:
                    var totalProductIcon = _unitOfWork.BoxItemRepository.EnableQuery()!.Where(c => c.BoxId == data.BoxId && c.BoxItemType == BoxItemType.ProductIcon).Count();
                    if (totalProductIcon >= box.MaxProductItem) throw new Exception($"BoxItem type product icon already reach max product item :{box.MaxProductItem}");
                    data.Order = totalProductIcon + 1;
                    break;

                case BoxItemType.Content:
                    data.BoxItemX = box.BoxPositionX;
                    data.BoxItemY = box.BoxPositionY;
                    data.BoxItemWidth = box.BoxWidth;
                    data.BoxItemHeight = box.BoxHeight;
                    data.Order = 0;
                    break;

                default:
                    break;
            }

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
                .Include(c => c.BFont);
            var result = DataQuery(data, boxItemId, boxId, fontId, searchString, pageNumber, pageSize);

            return result;
        }

        public BoxItem Update(int boxItemId, BoxItemUpdateDTO boxItemUpdateDTO)
        {
            var data = _unitOfWork.BoxItemRepository.Find(c => c.BoxItemId == boxItemId && c.IsDeleted == false).FirstOrDefault()
            ?? throw new Exception("BoxItem not found or deleted");

            var font = _unitOfWork.FontRepository.Find(c => c.BFontId == boxItemUpdateDTO.BFontId && c.IsDeleted == false).FirstOrDefault()
            ?? throw new Exception("Font not found or deleted");

            _mapper.Map(boxItemUpdateDTO, data);

            _unitOfWork.BoxItemRepository.Update(data);
            _unitOfWork.Save();

            return data;
        }

        private static IEnumerable<BoxItem> DataQuery(IQueryable<BoxItem> data, int? boxItemId, int? boxId, int? fontId, string? searchString, int pageNumber, int pageSize)
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
                data = data.Where(c => c.BFontId == fontId);
            }

            if (searchString != null)
            {
                if (Enum.TryParse(typeof(BoxItemType), searchString, out var boxItemType))
                {
                    data = data.Where(c => c.BoxItemType.Equals((BoxItemType)boxItemType!));
                }

                if (double.TryParse(searchString, out double fontSize))
                {
                    data = data.Where(c => c.BoxItemType.Equals((BoxItemType)Enum.Parse(typeof(BoxItemType), searchString)));
                }
            }

            return PaginatedList<BoxItem>.Create(data, pageNumber, pageSize);
        }
    }
}