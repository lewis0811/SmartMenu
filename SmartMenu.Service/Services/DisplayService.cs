using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SmartMenu.DAO;
using SmartMenu.Domain.Models;
using SmartMenu.Domain.Models.DTO;
using SmartMenu.Domain.Repository;
using SmartMenu.Service.Interfaces;

namespace SmartMenu.Service.Services
{
    public class DisplayService : IDisplayService
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;

        public DisplayService(IMapper mapper, IUnitOfWork unitOfWork)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        public IEnumerable<Display> GetAll(int? displayId, int? menuId, int? collectionId, string? searchString, int pageNumber, int pageSize)
        {
            var data = _unitOfWork.DisplayRepository.EnableQuery()
                .Include(c => c.Menu)
                .Include(c => c.Collection);
            var result = DataQuery(data, displayId, menuId, collectionId, searchString, pageNumber, pageSize);

            return result ?? Enumerable.Empty<Display>();
        }

        public IEnumerable<Display> GetWithItems(int? displayId, int? menuId, int? collectionId, string? searchString, int pageNumber, int pageSize)
        {
            var data = _unitOfWork.DisplayRepository.EnableQuery()
                .Include(c => c.DisplayItems)!
                .ThenInclude(c => c.ProductGroup)!
                .ThenInclude(c => c!.ProductGroupItems)!
                .ThenInclude(c => c!.Product)
                .ThenInclude(c => c!.ProductSizePrices)
                .Include(c => c.DisplayItems)!
                .ThenInclude(c => c.Box)
                .ThenInclude(c => c!.BoxItems);

            var result = DataQuery(data, displayId, menuId, collectionId, searchString, pageNumber, pageSize);

            return result ?? Enumerable.Empty<Display>();
        }

        public Display AddDisplay(DisplayCreateDTO displayCreateDTO)
        {
            if (displayCreateDTO.MenuId != null && displayCreateDTO.CollectionId != null) throw new Exception("MenuId and CollectionId cannot be using at the same time");

            var storeDevice = _unitOfWork.StoreDeviceRepository.Find(c => c.StoreDeviceId == displayCreateDTO.StoreDeviceId && c.IsDeleted == false).FirstOrDefault();
            var menu = _unitOfWork.MenuRepository.Find(c => c.MenuId == displayCreateDTO.MenuId && c.IsDeleted == false).FirstOrDefault();
            var collection = _unitOfWork.CollectionRepository.Find(c => c.CollectionId == displayCreateDTO.CollectionId && c.IsDeleted == false).FirstOrDefault();
            var template = _unitOfWork.TemplateRepository.Find(c => c.TemplateId == displayCreateDTO.TemplateId && c.IsDeleted == false).FirstOrDefault();

            if (storeDevice == null) throw new Exception("StoreDevice not found or deleted");
            if (menu == null) throw new Exception("Menu not found or deleted");
            if (collection == null) throw new Exception("Collection not found or deleted");
            if (template == null) throw new Exception("Template not found or deleted");

            var data = _mapper.Map<Display>(displayCreateDTO);
            _unitOfWork.DisplayRepository.Add(data);
            _unitOfWork.Save();

            return data;
        }

        public Display AddDisplayV2(DisplayCreateDTO displayCreateDTO)
        {
            if (displayCreateDTO.MenuId == 0) displayCreateDTO.MenuId = null;
            if (displayCreateDTO.CollectionId == 0) displayCreateDTO.CollectionId = null;

            var storeDevice = _unitOfWork.StoreDeviceRepository.Find(c => c.StoreDeviceId == displayCreateDTO.StoreDeviceId && c.IsDeleted == false).FirstOrDefault();
            var template = _unitOfWork.TemplateRepository.Find(c => c.TemplateId == displayCreateDTO.TemplateId && c.IsDeleted == false).FirstOrDefault();

            var menu = new Menu();
            if (displayCreateDTO.MenuId != null)
            {
                menu = _unitOfWork.MenuRepository.Find(c => c.MenuId == displayCreateDTO.MenuId && c.IsDeleted == false).FirstOrDefault();
            }
            else { menu = null; }

            var collection = new Collection();
            if (displayCreateDTO.CollectionId != null)
            {
                collection = _unitOfWork.CollectionRepository.Find(c => c.CollectionId == displayCreateDTO.CollectionId && c.IsDeleted == false).FirstOrDefault();
            }
            else { collection = null; }

            if (storeDevice == null) throw new Exception("StoreDevice not found or deleted");
            if (menu == null && collection == null) throw new Exception("Menu/Collection not found or deleted");
            //if (collection == null && displayCreateDTO.CollectionId != 0) return BadRequest("Collection not found or deleted");
            if (template == null) throw new Exception("Template not found or deleted");

            var data = _mapper.Map<Display>(displayCreateDTO);
            _unitOfWork.DisplayRepository.Add(data);
            _unitOfWork.Save();

            // Adding display items
            var productGroups = new List<ProductGroup>();
            var boxes = new List<Box>();
            var layers = new List<Layer>();
            var templateWithLayer = new Template();
            var displayItems = new List<DisplayItem>();

            // GET ProductGroup List from Menu or Collection if not null
            if (menu != null)
            {
                productGroups = _unitOfWork.ProductGroupRepository.GetProductGroup(null, menu.MenuId, null);
            }

            if (collection != null)
            {
                productGroups = _unitOfWork.ProductGroupRepository.GetProductGroup(null, null, collection.CollectionId);
            }

            // GET Box List from display's template
            templateWithLayer = _unitOfWork.TemplateRepository.GetTemplateWithLayersAndBoxes(template.TemplateId);

            if (templateWithLayer.Layers != null)
            {
                layers.AddRange(templateWithLayer.Layers);

                foreach (var layer in layers)
                {
                    if (layer.Boxes != null)
                    {
                        boxes.AddRange(layer.Boxes);
                    }
                }

                // Query exact box in needed
                boxes = boxes.Where(c => c.BoxType == Domain.Models.Enum.BoxType.UseInDisplay).ToList();
            }

            // Get display items list from product groups and boxes
            int productGroupCount = productGroups.Count;
            int boxCount = boxes.Count;
            //var boxesToRender = boxes.Where(c => c.)

            if (boxCount < productGroupCount)
            {
                _unitOfWork.DisplayRepository.Remove(data);
                _unitOfWork.Save();

                throw new Exception("Not enough boxes for rendering product groups.");
            }

            // Adding display items to database
            for (int i = 0; i < productGroupCount; i++)
            {
                DisplayItemCreateDTO item = new()
                {
                    DisplayID = data.DisplayId,
                    BoxID = boxes[i].BoxId,
                    ProductGroupID = productGroups[i].ProductGroupId
                };

                var itemData = _mapper.Map<DisplayItem>(item);
                _unitOfWork.DisplayItemRepository.Add(itemData);
                _unitOfWork.Save();
            }

            return data;
        }

        public Display Update(int displayId, DisplayUpdateDTO displayUpdateDTO)
        {
            var menu = _unitOfWork.MenuRepository.Find(c => c.MenuId == displayUpdateDTO.MenuId && c.IsDeleted == false).FirstOrDefault();
            if (menu == null) throw new Exception("Menu not found or deleted");

            var collection = _unitOfWork.CollectionRepository.Find(c => c.CollectionId == displayUpdateDTO.CollectionId && c.IsDeleted == false).FirstOrDefault();
            if (collection == null) throw new Exception("Collection not found or deleted");

            var template = _unitOfWork.TemplateRepository.Find(c => c.TemplateId == displayUpdateDTO.TemplateId && c.IsDeleted == false).FirstOrDefault();
            if (template == null) throw new Exception("Template not found or deleted");

            var data = _unitOfWork.DisplayRepository.Find(c => c.DisplayId == displayId && c.IsDeleted == false).FirstOrDefault();
            if (data == null) throw new Exception("Display not found or deleted");

            _mapper.Map(displayUpdateDTO, data);
            _unitOfWork.DisplayRepository.Update(data);
            _unitOfWork.Save();

            return data;
        }

        public void Delete(int displayId)
        {
            var data = _unitOfWork.DisplayRepository.Find(c => c.DisplayId == displayId && c.IsDeleted == false).FirstOrDefault();
            if (data == null) throw new Exception("Display not found or deleted");
            _unitOfWork.DisplayRepository.Remove(data);
            _unitOfWork.Save();
        }

        private static IEnumerable<Display> DataQuery(IQueryable<Display> data, int? displayId, int? menuId, int? collectionId, string? searchString, int pageNumber, int pageSize)
        {
            data = data.Where(c => c.IsDeleted == false);

            if (displayId != null)
            {
                data = data.Where(c => c.DisplayId == displayId);
            }

            if (menuId != null)
            {
                data = data.Where(c => c.MenuId == menuId);
            }

            if (collectionId != null)
            {
                data = data.Where(c => c.CollectionId == collectionId);
            }

            if (searchString != null)
            {
                data = data.Where(c => c.ActiveHour.ToString().Contains(searchString));
            }

            return PaginatedList<Display>.Create(data, pageNumber, pageSize);
        }
    }
}