using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using SmartMenu.Domain.Models;
using SmartMenu.Domain.Models.DTO;
using SmartMenu.Domain.Repository;

namespace SmartMenu.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BoxItemController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;

        public BoxItemController(IMapper mapper, IUnitOfWork unitOfWork)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        public IActionResult Get(int? boxItemId, int? boxId, int? fontId, string? searchString, int pageNumber = 1, int pageSize = 10)
        {
            var data = _unitOfWork.BoxItemRepository
                .GetAll(boxItemId, boxId, fontId, searchString, pageNumber, pageSize);
            
            data ??= Enumerable.Empty<BoxItem>();
            return Ok(data);
        }

        [HttpPost]
        public IActionResult Add(BoxItemCreateDTO boxItemCreateDTO)
        {
            var box = _unitOfWork.BoxRepository.Find(c => c.BoxID == boxItemCreateDTO.BoxId && c.IsDeleted == false).FirstOrDefault();
            if (box == null) return BadRequest("Box not found or deleted");

            var font = _unitOfWork.FontRepository.Find(c => c.FontID == boxItemCreateDTO.FontId && c.IsDeleted == false).FirstOrDefault();
            if (font == null) return BadRequest("Font not found or deleted");

            var data = _mapper.Map<BoxItem>(boxItemCreateDTO);

            _unitOfWork.BoxItemRepository.Add(data);
            _unitOfWork.Save();

            return CreatedAtAction(nameof(Get), data);
        }

        [HttpPut("{boxItemId}")]
        public IActionResult Update(int boxItemId, BoxItemUpdateDTO boxItemUpdateDTO)
        {
            var data = _unitOfWork.BoxItemRepository.Find(c => c.BoxItemId == boxItemId && c.IsDeleted == false).FirstOrDefault();
            if (data == null) return BadRequest("BoxItem not found or deleted");

            var font = _unitOfWork.FontRepository.Find(c => c.FontID == boxItemUpdateDTO.FontId && c.IsDeleted == false).FirstOrDefault();
            if (font == null) return BadRequest("Font not found or deleted");

            _mapper.Map(boxItemUpdateDTO, data);

            _unitOfWork.BoxItemRepository.Update(data);
            _unitOfWork.Save();

            return Ok(data);
        }

        [HttpDelete("{boxItemId}")]
        public IActionResult Delete(int boxItemId)
        {
            var data = _unitOfWork.BoxItemRepository.Find(c => c.BoxItemId == boxItemId && c.IsDeleted == false).FirstOrDefault();
            if (data == null) return BadRequest("BoxItem not found or deleted");

            data.IsDeleted = true;
            _unitOfWork.BoxItemRepository.Update(data);
            _unitOfWork.Save();

            return Ok(data);
        }
    }
}