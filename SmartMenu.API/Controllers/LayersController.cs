using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using SmartMenu.Domain.Models;
using SmartMenu.Domain.Models.DTO;
using SmartMenu.Domain.Repository;

namespace SmartMenu.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LayersController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public LayersController(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        [HttpGet]
        public IActionResult Get(int? layerId, int? templateId, string? searchString, int pageNumber = 1, int pageSize = 10)
        {
            var data = _unitOfWork.LayerRepository.GetAll(layerId, templateId, searchString, pageNumber, pageSize).ToList();
            if (data.Count == 0) return NotFound();

            return Ok(data);
        }

        [HttpGet("LayerItems")]
        public IActionResult GetLayerItems(int? layerId, int? templateId, string? searchString, int pageNumber = 1, int pageSize = 10)
        {
            var data = _unitOfWork.LayerRepository.GetAllWithLayerItems(layerId, templateId, searchString, pageNumber, pageSize).ToList();
            if (data.Count == 0) return NotFound();

            return Ok(data);
        }

        [HttpGet("Boxes")]
        public IActionResult GetWithBoxes(int? layerId, int? templateId, string? searchString, int pageNumber = 1, int pageSize = 10)
        {
            var data = _unitOfWork.LayerRepository.GetAllWithBoxes(layerId, templateId, searchString, pageNumber, pageSize).ToList();
            if (data.Count == 0) return NotFound();

            return Ok(data);
        }

        [HttpPost]
        public IActionResult Add(LayerCreateDTO layerCreateDTO)
        {
            var data = _mapper.Map<Layer>(layerCreateDTO);

            _unitOfWork.LayerRepository.Add(data);
            _unitOfWork.Save();

            return CreatedAtAction(nameof(Get),  data);
        }

        [HttpPut("{layerId}")]
        public IActionResult Update(int layerId, LayerUpdateDTO layerUpdateDTO)
        {
            var data = _unitOfWork.LayerRepository.Find(c => c.LayerID == layerId).FirstOrDefault();
            if (data == null || data.IsDeleted == true) return NotFound();

            _mapper.Map(layerUpdateDTO, data);
            _unitOfWork.LayerRepository.Update(data);
            _unitOfWork.Save();

            return Ok(data);
        }

        [HttpDelete("{layerId}")]
        public IActionResult Delete(int layerId)
        {
            var data = _unitOfWork.LayerRepository.Find(c => c.LayerID == layerId).FirstOrDefault();
            if (data == null || data.IsDeleted == true) return NotFound();

            data.IsDeleted = true;

            _unitOfWork.LayerRepository.Update(data);
            _unitOfWork.Save();
            return Ok();
        }
    }
}