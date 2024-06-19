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
            var data = _unitOfWork.LayerRepository.GetAll(layerId, templateId, searchString, pageNumber, pageSize);
            data ??= Enumerable.Empty<Layer>();

            return Ok(data);
        }

        [HttpGet("LayerItems")]
        public IActionResult GetLayerItems(int? layerId, int? templateId, string? searchString, int pageNumber = 1, int pageSize = 10)
        {
            var data = _unitOfWork.LayerRepository.GetAllWithLayerItems(layerId, templateId, searchString, pageNumber, pageSize);
            data ??= Enumerable.Empty<Layer>();

            return Ok(data);
        }

        [HttpGet("Boxes")]
        public IActionResult GetWithBoxes(int? layerId, int? templateId, string? searchString, int pageNumber = 1, int pageSize = 10)
        {
            var data = _unitOfWork.LayerRepository.GetAllWithBoxes(layerId, templateId, searchString, pageNumber, pageSize);
            data ??= Enumerable.Empty<Layer>();

            return Ok(data);
        }

        [HttpPost]
        public IActionResult Add(LayerCreateDTO layerCreateDTO)
        {
            var template = _unitOfWork.TemplateRepository.Find(c => c.TemplateID == layerCreateDTO.TemplateID && c.IsDeleted == false).FirstOrDefault();
            if (template == null) return BadRequest("Template not found or deleted");
            var data = _mapper.Map<Layer>(layerCreateDTO);

            _unitOfWork.LayerRepository.Add(data);
            _unitOfWork.Save();

            return CreatedAtAction(nameof(Get), data);
        }

        [HttpPut("{layerId}")]
        public IActionResult Update(int layerId, LayerUpdateDTO layerUpdateDTO)
        {
            var data = _unitOfWork.LayerRepository.Find(c => c.LayerID == layerId && c.IsDeleted == false).FirstOrDefault();
            if (data == null) return BadRequest("Template not found or deleted");

            _mapper.Map(layerUpdateDTO, data);
            _unitOfWork.LayerRepository.Update(data);
            _unitOfWork.Save();

            return Ok(data);
        }

        [HttpDelete("{layerId}")]
        public IActionResult Delete(int layerId)
        {
            var data = _unitOfWork.LayerRepository.Find(c => c.LayerID == layerId && c.IsDeleted == false).FirstOrDefault();
            if (data == null) return BadRequest("Template not found or deleted");

            data.IsDeleted = true;

            _unitOfWork.LayerRepository.Update(data);
            _unitOfWork.Save();
            return Ok();
        }
    }
}