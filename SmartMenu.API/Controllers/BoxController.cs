using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using SmartMenu.Domain.Models;
using SmartMenu.Domain.Models.DTO;
using SmartMenu.Domain.Repository;

namespace SmartMenu.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BoxController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;

        public BoxController(IMapper mapper, IUnitOfWork unitOfWork)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        public ActionResult Get(
            int? boxId,
            int? layerId,
            int? fontId,
            string? searchString,
            int pageNumber = 1,
            int pageSize = 10)
        {
            try
            {
                var data = _unitOfWork.BoxRepository
                    .GetAll(boxId, layerId, fontId, searchString, pageNumber, pageSize)
                    .ToList();
                if (data.Count == 0) return NotFound();

                return Ok(data);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        public IActionResult Add(BoxCreateDTO boxCreateDTO)
        {
            var data = _mapper.Map<Box>(boxCreateDTO);

            _unitOfWork.BoxRepository.Add(data);
            _unitOfWork.Save();

            return CreatedAtAction(nameof(Get), data);
        }

        [HttpPut("{boxId}")]
        public IActionResult Update(int boxId, BoxUpdateDTO boxUpdateDTO)
        {
            var data = _unitOfWork.BoxRepository.Find(c => c.BoxID == boxId).FirstOrDefault();
            if (data == null || data.IsDeleted == true) return NotFound();

            _mapper.Map(boxUpdateDTO, data);
            _unitOfWork.BoxRepository.Update(data);
            _unitOfWork.Save();

            return Ok(data);
        }

        [HttpDelete("{boxId}")]
        public IActionResult Delete(int boxId)
        {
            var data = _unitOfWork.BoxRepository.Find(c => c.BoxID == boxId).FirstOrDefault();
            if (data == null || data.IsDeleted == true) return NotFound();

            data.IsDeleted = true;
            _unitOfWork.BoxRepository.Update(data);
            _unitOfWork.Save();

            return Ok();
        }
    }
}