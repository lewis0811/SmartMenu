using AutoMapper;
using Microsoft.AspNetCore.Mvc;
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

        [HttpPost]
        public IActionResult Add(DisplayCreateDTO displayCreateDTO)
        {
            var storeDevice = _unitOfWork.StoreDeviceRepository.Find(c => c.StoreDeviceId == displayCreateDTO.StoreDeviceId && c.IsDeleted == false).FirstOrDefault();
            if (storeDevice == null) return BadRequest("StoreDevice not found or deleted");

            var menu = _unitOfWork.MenuRepository.Find(c => c.MenuID == displayCreateDTO.MenuId && c.IsDeleted == false).FirstOrDefault();
            if (menu == null) return BadRequest("Menu not found or deleted");

            var collection = _unitOfWork.CollectionRepository.Find(c => c.CollectionID == displayCreateDTO.CollectionId && c.IsDeleted == false).FirstOrDefault();
            if (collection == null) return BadRequest("Collection not found or deleted");

            var template = _unitOfWork.TemplateRepository.Find(c => c.TemplateID == displayCreateDTO.TemplateId && c.IsDeleted == false).FirstOrDefault();
            if (template == null) return BadRequest("Template not found or deleted");

            var data = _mapper.Map<Display>(displayCreateDTO);
            _unitOfWork.DisplayRepository.Add(data);
            _unitOfWork.Save();
            return CreatedAtAction(nameof(Get), data);
        }

        [HttpPut("{displayId}")]
        public IActionResult Update(int displayId, DisplayUpdateDTO displayUpdateDTO)
        {
            var menu = _unitOfWork.MenuRepository.Find(c => c.MenuID == displayUpdateDTO.MenuId && c.IsDeleted == false).FirstOrDefault();
            if (menu == null) return BadRequest("Menu not found or deleted");

            var collection = _unitOfWork.CollectionRepository.Find(c => c.CollectionID == displayUpdateDTO.CollectionId && c.IsDeleted == false).FirstOrDefault();
            if (collection == null) return BadRequest("Collection not found or deleted");

            var template = _unitOfWork.TemplateRepository.Find(c => c.TemplateID == displayUpdateDTO.TemplateId && c.IsDeleted == false).FirstOrDefault();
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