using Microsoft.AspNetCore.Mvc;
using SmartMenu.Domain.Models.DTO;
using SmartMenu.Service.Interfaces;

namespace SmartMenu.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BoxesController : ControllerBase
    {
        private readonly IBoxService _boxService;

        public BoxesController(IBoxService boxService)
        {
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
                return BadRequest(new { error = ex.Message });
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
                return BadRequest(new { error = ex.Message });
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
                return BadRequest(new { error = ex.Message });
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
                return BadRequest(new { error = ex.Message });
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
                return BadRequest(new { error = ex.Message });
            }
        }
    }
}