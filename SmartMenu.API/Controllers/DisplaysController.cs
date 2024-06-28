using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartMenu.Domain.Models;
using SmartMenu.Domain.Models.DTO;
using SmartMenu.Domain.Repository;

namespace SmartMenu.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DisplaysController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;

        public DisplaysController(IMapper mapper, IUnitOfWork unitOfWork)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        public IActionResult Get(int? displayId, int? menuId, int? collectionId, string? searchString, int pageNumber = 1, int pageSize = 10)
        {
            var data = _unitOfWork.DisplayRepository.GetAll(displayId, menuId, collectionId, searchString, pageNumber, pageSize);
            data ??= Enumerable.Empty<Display>();

            return Ok(data);
        }

        [HttpGet("DisplayItems")]
        public IActionResult GetWithItems(int? displayId, int? menuId, int? collectionId, string? searchString, int pageNumber = 1, int pageSize = 10)
        {
            var data = _unitOfWork.DisplayRepository.GetWithItems(displayId, menuId, collectionId, searchString, pageNumber, pageSize);
            data ??= Enumerable.Empty<Display>();

            return Ok(data);
        }

        [HttpPost]
        public IActionResult Add(DisplayCreateDTO displayCreateDTO)
        {
            if (displayCreateDTO.MenuId != null && displayCreateDTO.CollectionId != null) return BadRequest("MenuId and CollectionId cannot be using at the same time");

            var storeDevice = _unitOfWork.StoreDeviceRepository.Find(c => c.StoreDeviceId == displayCreateDTO.StoreDeviceId && c.IsDeleted == false).FirstOrDefault();
            var menu = _unitOfWork.MenuRepository.Find(c => c.MenuId == displayCreateDTO.MenuId && c.IsDeleted == false).FirstOrDefault();
            var collection = _unitOfWork.CollectionRepository.Find(c => c.CollectionId == displayCreateDTO.CollectionId && c.IsDeleted == false).FirstOrDefault();
            var template = _unitOfWork.TemplateRepository.Find(c => c.TemplateId == displayCreateDTO.TemplateId && c.IsDeleted == false).FirstOrDefault();

            if (storeDevice == null) return BadRequest("StoreDevice not found or deleted");
            if (menu == null) return BadRequest("Menu not found or deleted");
            if (collection == null) return BadRequest("Collection not found or deleted");
            if (template == null) return BadRequest("Template not found or deleted");

            var data = _mapper.Map<Display>(displayCreateDTO);
            _unitOfWork.DisplayRepository.Add(data);
            _unitOfWork.Save();
            return CreatedAtAction(nameof(Get), data);
        }

        [HttpPost("V2")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public IActionResult AddV2(DisplayCreateDTO displayCreateDTO)
        {
            try
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

                if (storeDevice == null) return BadRequest("StoreDevice not found or deleted");
                if (menu == null && collection == null) return BadRequest("Menu/Collection not found or deleted");
                //if (collection == null && displayCreateDTO.CollectionId != 0) return BadRequest("Collection not found or deleted");
                if (template == null) return BadRequest("Template not found or deleted");

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

                    return BadRequest("Not enough boxes for rendering product groups.");
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

                return CreatedAtAction(nameof(Get), new {displayId = data.DisplayId});
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{displayId}")]
        public IActionResult Update(int displayId, DisplayUpdateDTO displayUpdateDTO)
        {
            var menu = _unitOfWork.MenuRepository.Find(c => c.MenuId == displayUpdateDTO.MenuId && c.IsDeleted == false).FirstOrDefault();
            if (menu == null) return BadRequest("Menu not found or deleted");

            var collection = _unitOfWork.CollectionRepository.Find(c => c.CollectionId == displayUpdateDTO.CollectionId && c.IsDeleted == false).FirstOrDefault();
            if (collection == null) return BadRequest("Collection not found or deleted");

            var template = _unitOfWork.TemplateRepository.Find(c => c.TemplateId == displayUpdateDTO.TemplateId && c.IsDeleted == false).FirstOrDefault();
            if (template == null) return BadRequest("Template not found or deleted");

            var data = _unitOfWork.DisplayRepository.Find(c => c.DisplayId == displayId && c.IsDeleted == false).FirstOrDefault();
            if (data == null) return BadRequest("Display not found or deleted");

            _mapper.Map(displayUpdateDTO, data);
            _unitOfWork.DisplayRepository.Update(data);
            _unitOfWork.Save();
            return Ok(data);
        }

        [HttpDelete("{displayId}")]
        public IActionResult Delete(int displayId)
        {
            var data = _unitOfWork.DisplayRepository.Find(c => c.DisplayId == displayId && c.IsDeleted == false).FirstOrDefault();
            if (data == null) return BadRequest("Display not found or deleted");
            _unitOfWork.DisplayRepository.Remove(data);
            _unitOfWork.Save();
            return Ok();
        }
    }
}