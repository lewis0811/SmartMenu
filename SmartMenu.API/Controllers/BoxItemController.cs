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
            var data = _mapper.Map<BoxItem>(boxItemCreateDTO);

            _unitOfWork.BoxItemRepository.Add(data);
            _unitOfWork.Save();

            return CreatedAtAction(nameof(Get), data);
        }

        [HttpPut("{boxItemId}")]
        public IActionResult Update(int boxItemId, BoxItemUpdateDTO boxItemUpdateDTO)
        {
            var data = _unitOfWork.BoxItemRepository.Find(c => c.BoxItemId == boxItemId).FirstOrDefault();
            if (data == null || data.IsDeleted == true) return NotFound();

            _mapper.Map(boxItemUpdateDTO, data);

            _unitOfWork.BoxItemRepository.Update(data);
            _unitOfWork.Save();

            return Ok(data);
        }

        [HttpDelete("{boxItemId}")]
        public IActionResult Delete(int boxItemId)
        {
            var data = _unitOfWork.BoxItemRepository.Find(c => c.BoxItemId == boxItemId).FirstOrDefault();
            if (data == null || data.IsDeleted == true) return NotFound();

            data.IsDeleted = true;
            _unitOfWork.BoxItemRepository.Update(data);
            _unitOfWork.Save();

            return Ok(data);
        }
    }
}