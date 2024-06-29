using Microsoft.AspNetCore.Mvc;
using SmartMenu.Domain.Models.DTO;
using SmartMenu.Service.Interfaces;

namespace SmartMenu.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BoxItemController : ControllerBase
    {
        private readonly IBoxItemService _boxItemService;

        public BoxItemController(IBoxItemService boxItemService)
        {
            _boxItemService = boxItemService;
        }

        [HttpGet]
        public IActionResult Get(int? boxItemId, int? boxId, int? fontId, string? searchString, int pageNumber = 1, int pageSize = 10)
        {
            try
            {
                var data = _boxItemService.GetAll(boxItemId, boxId, fontId, searchString, pageNumber, pageSize);
                return Ok(data);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        public IActionResult Add(BoxItemCreateDTO boxItemCreateDTO)
        {
            try
            {
                var data = _boxItemService.AddBoxItem(boxItemCreateDTO);
                return CreatedAtAction(nameof(Get), data);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{boxItemId}")]
        public IActionResult Update(int boxItemId, BoxItemUpdateDTO boxItemUpdateDTO)
        {
            try
            {
                var data = _boxItemService.Update(boxItemId, boxItemUpdateDTO);
                return Ok(data);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("{boxItemId}")]
        public IActionResult Delete(int boxItemId)
        {
            try
            {
                _boxItemService.Delete(boxItemId);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}