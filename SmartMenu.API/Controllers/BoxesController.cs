using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using SmartMenu.Domain.Models.DTO;
using SmartMenu.Domain.Repository;
using SmartMenu.Service.Interfaces;

namespace SmartMenu.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BoxesController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IBoxService _boxService;

        public BoxesController(IMapper mapper, IUnitOfWork unitOfWork, IBoxService boxService)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _boxService = boxService;
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
                var data = _boxService.GetAll(boxId, layerId, searchString, pageNumber, pageSize);
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
                var data = _boxService.GetAllWithBoxItems(boxId, layerId, searchString, pageNumber, pageSize);
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
            try
            {
                var data = _boxService.Add(boxCreateDTO);
                return CreatedAtAction(nameof(Get), data);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{boxId}")]
        public IActionResult Update(int boxId, BoxUpdateDTO boxUpdateDTO)
        {
            try
            {
                var data = _boxService.Update(boxId, boxUpdateDTO);
                return Ok(data);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("{boxId}")]
        public IActionResult Delete(int boxId)
        {
            try
            {
                _boxService.Delete(boxId);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}