using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using SmartMenu.Domain.Models;
using SmartMenu.Domain.Models.DTO;
using SmartMenu.Domain.Repository;

namespace SmartMenu.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LayerItemsController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public LayerItemsController(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        [HttpGet]
        public IActionResult Get(int? layerItemId, int? layerId, string? searchString, int pageNumber = 1, int pageSize = 10)
        {
            var data = _unitOfWork.LayerItemRepository.GetAll(layerItemId, layerId, searchString, pageNumber, pageSize).ToList();
            if (data.Count == 0) return NotFound();

            return Ok(data);
        }

        [HttpPost]
        public IActionResult Add(LayerItemCreateDTO layerItemCreateDTO)
        {
            var data = _mapper.Map<LayerItem>(layerItemCreateDTO);

            _unitOfWork.LayerItemRepository.Add(data);
            _unitOfWork.Save();

            return CreatedAtAction(nameof(Get), data);
        }

        [HttpPut("{layerItemId}")]
        public IActionResult Update(int layerItemId, LayerItemUpdateDTO layerItemUpdateDTO)
        {
            var data = _unitOfWork.LayerItemRepository.Find(c => c.LayerItemID == layerItemId).FirstOrDefault();
            if (data == null || data.IsDeleted == true) return NotFound();

            _mapper.Map(layerItemUpdateDTO, data);
            _unitOfWork.LayerItemRepository.Update(data);
            _unitOfWork.Save();

            return Ok(data);
        }

        [HttpDelete("{layerItemId}")]
        public IActionResult Delete(int layerItemId)
        {
            var data = _unitOfWork.LayerItemRepository.Find(c => c.LayerItemID == layerItemId).FirstOrDefault();
            if (data == null || data.IsDeleted == true) return NotFound();

            data.IsDeleted = true;

            _unitOfWork.LayerItemRepository.Update(data);
            _unitOfWork.Save();

            return Ok(data);
        }
    }
}