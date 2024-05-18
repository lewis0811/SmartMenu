using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using SmartMenu.Domain.Models;
using SmartMenu.Domain.Models.DTO;
using SmartMenu.Domain.Repository;

namespace SmartMenu.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TemplateController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;

        public TemplateController(IMapper mapper, IUnitOfWork unitOfWork)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        public ActionResult Get(int? templateId, string? searchString, int pageNumber = 1, int pageSize = 10)
        {
            var data = _unitOfWork.TemplateRepository.GetAll(templateId, searchString, pageNumber, pageSize).ToList();
            if (data.Count == 0) return NotFound();

            return Ok(data);
        }

        [HttpGet("Layers")]
        public ActionResult GetLayers(int? templateId, string? searchString, int pageNumber = 1, int pageSize = 10)
        {
            var data = _unitOfWork.TemplateRepository.GetAllWithLayers(templateId, searchString, pageNumber, pageSize).ToList();
            if (data.Count == 0) return NotFound();

            return Ok(data);
        }

        [HttpPost]
        public IActionResult Add(TemplateCreateDTO templateCreateDTO)
        {
            var data = _mapper.Map<Template>(templateCreateDTO);
            _unitOfWork.TemplateRepository.Add(data);
            _unitOfWork.Save();
            return CreatedAtAction(nameof(Get), data);
        }

        [HttpPut]
        public IActionResult Update(int templateId, TemplateCreateDTO templateUpdateDTO)
        {
            var data = _unitOfWork.TemplateRepository.Find(c => c.TemplateID == templateId).FirstOrDefault();
            if (data == null || data.IsDeleted == true) return NotFound();

            _mapper.Map(templateUpdateDTO, data);

            _unitOfWork.TemplateRepository.Update(data);
            _unitOfWork.Save();

            return Ok();
        }

        [HttpDelete]
        public IActionResult Delete(int templateId)
        {
            var data = _unitOfWork.TemplateRepository.Find(c => c.TemplateID == templateId).FirstOrDefault();
            if (data == null || data.IsDeleted == true) return NotFound();

            data.IsDeleted = true;

            _unitOfWork.TemplateRepository.Update(data);
            _unitOfWork.Save();

            return Ok();
        }
    }
}