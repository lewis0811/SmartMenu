using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using SmartMenu.Domain.Models;
using SmartMenu.Domain.Models.DTO;
using SmartMenu.Domain.Repository;

namespace SmartMenu.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DisplayItemsController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;

        public DisplayItemsController(IMapper mapper, IUnitOfWork unitOfWork)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        public IActionResult Get(int displayItemId, int displayId, int boxId, int productGroupId, string? searchString, int pageNumber = 1, int pageSize = 10)
        {
            var data = _unitOfWork.DisplayItemRepository
                .GetAll(displayItemId, displayId, boxId, productGroupId, searchString, pageNumber, pageSize);
            data ??= Enumerable.Empty<DisplayItem>();

            return Ok(data);
        }

        [HttpPost]
        public IActionResult Add(DisplayItemCreateDTO displayItemCreateDTO)
        {
            var data = _mapper.Map<DisplayItem>(displayItemCreateDTO);
            _unitOfWork.DisplayItemRepository.Add(data);
            _unitOfWork.Save();
            return CreatedAtAction(nameof(Get), data);
        }

        [HttpPut("{displayItemId}")]
        public IActionResult Update(int displayItemId, DisplayItemUpdateDTO displayItemUpdateDTO)
        {
            var data = _mapper.Map<DisplayItem>(displayItemUpdateDTO);
            _unitOfWork.DisplayItemRepository.Update(data);
            _unitOfWork.Save();
            return Ok();
        }

        [HttpDelete("{displayItemId}")]
        public IActionResult Delete(int displayItemId)
        {
            var data = _unitOfWork.DisplayItemRepository.Find(c => c.DisplayItemId == displayItemId).FirstOrDefault();
            if (data == null) return BadRequest("DisplayItem not found or deleted");

            _unitOfWork.DisplayItemRepository.Remove(data);
            _unitOfWork.Save();
            return Ok();
        }
    }
}