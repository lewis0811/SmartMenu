using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using SmartMenu.Domain.Models;
using SmartMenu.Domain.Models.DTO;
using SmartMenu.Domain.Repository;

namespace SmartMenu.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TemplatesController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;

        public TemplatesController(IMapper mapper, IUnitOfWork unitOfWork)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        public ActionResult Get(int? templateId, string? searchString, int pageNumber = 1, int pageSize = 10)
        {
            var data = _unitOfWork.TemplateRepository.GetAll(templateId, searchString, pageNumber, pageSize);
            data ??= Enumerable.Empty<Template>();

            return Ok(data);
        }

        [HttpGet("Layers")]
        public ActionResult GetLayers(int? templateId, string? searchString, int pageNumber = 1, int pageSize = 10)
        {
            var data = _unitOfWork.TemplateRepository.GetAllWithLayers(templateId, searchString, pageNumber, pageSize);
            data ??= Enumerable.Empty<Template>();

            return Ok(data);
        }

        [HttpPost]
        public IActionResult Add(TemplateCreateDTO templateCreateDTO)
        {
            var brand = _unitOfWork.BrandRepository.Find(c => c.BrandId == templateCreateDTO.BrandId && c.IsDeleted == false).FirstOrDefault();
            if (brand == null) return BadRequest("Brand not found or deleted");

            var data = _mapper.Map<Template>(templateCreateDTO);
            _unitOfWork.TemplateRepository.Add(data);
            _unitOfWork.Save();
            return CreatedAtAction(nameof(Get), data);
        }

        [HttpPut("{templateId}")]
        public IActionResult Update(int templateId, TemplateUpdateDTO templateUpdateDTO)
        {
            var data = _unitOfWork.TemplateRepository.Find(c => c.TemplateID == templateId && c.IsDeleted == false).FirstOrDefault();
            if (data == null) return BadRequest("Template not found or deleted");

            _mapper.Map(templateUpdateDTO, data);

            _unitOfWork.TemplateRepository.Update(data);
            _unitOfWork.Save();

            return Ok();
        }

        [HttpDelete("{templateId}")]
        public IActionResult Delete(int templateId)
        {
            var data = _unitOfWork.TemplateRepository.Find(c => c.TemplateID == templateId && c.IsDeleted == false).FirstOrDefault();
            if (data == null) return BadRequest("Template not found or deleted");

            data.IsDeleted = true;

            _unitOfWork.TemplateRepository.Update(data);
            _unitOfWork.Save();

            return Ok();
        }
    }
}