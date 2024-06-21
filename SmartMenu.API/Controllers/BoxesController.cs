using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using SmartMenu.Domain.Models;
using SmartMenu.Domain.Models.DTO;
using SmartMenu.Domain.Repository;

namespace SmartMenu.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BoxesController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;

        public BoxesController(IMapper mapper, IUnitOfWork unitOfWork)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        public ActionResult Get(
            int? boxId,
            int? layerId,
            string? searchString,
            int pageNumber = 1,
            int pageSize = 10)
        {
            try
            {
                var data = _unitOfWork.BoxRepository
                    .GetAll(boxId, layerId, searchString, pageNumber, pageSize);
                data ??= Enumerable.Empty<Box>();

                return Ok(data);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("BoxItems")]
        public ActionResult GetItems(
            int? boxId,
            int? layerId,
            string? searchString,
            int pageNumber = 1,
            int pageSize = 10)
        {
            try
            {
                var data = _unitOfWork.BoxRepository
                    .GetAllWithBoxItems(boxId, layerId, searchString, pageNumber, pageSize);
                data ??= Enumerable.Empty<Box>();

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
            var layer = _unitOfWork.LayerRepository.Find(c => c.LayerID == boxCreateDTO.LayerId && c.IsDeleted == false).FirstOrDefault();
            if (layer == null) return BadRequest("Layer not found or deleted");
            var data = _mapper.Map<Box>(boxCreateDTO);

            _unitOfWork.BoxRepository.Add(data);
            _unitOfWork.Save();

            return CreatedAtAction(nameof(Get), data);
        }

        [HttpPut("{boxId}")]
        public IActionResult Update(int boxId, BoxUpdateDTO boxUpdateDTO)
        {
            var data = _unitOfWork.BoxRepository.Find(c => c.BoxID == boxId && c.IsDeleted == false).FirstOrDefault();
            if (data == null) return BadRequest("Box not found or deleted");

            _mapper.Map(boxUpdateDTO, data);
            _unitOfWork.BoxRepository.Update(data);
            _unitOfWork.Save();

            return Ok(data);
        }

        [HttpDelete("{boxId}")]
        public IActionResult Delete(int boxId)
        {
            var data = _unitOfWork.BoxRepository.Find(c => c.BoxID == boxId && c.IsDeleted == false).FirstOrDefault();
            if (data == null) return BadRequest("Box not found or deleted");

            data.IsDeleted = true;
            _unitOfWork.BoxRepository.Update(data);
            _unitOfWork.Save();

            return Ok();
        }
    }
}