using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartMenu.API.Ultility;
using SmartMenu.Domain.Models;
using SmartMenu.Domain.Models.DTO;
using SmartMenu.Domain.Repository;
using SmartMenu.Service.Interfaces;

namespace SmartMenu.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = SD.Role_StoreManager)]
    public class DisplayItemsController : ControllerBase
    {
        private readonly IDisplayItemService _displayItemService;

        public DisplayItemsController(IDisplayItemService displayItemService)
        {
            _displayItemService = displayItemService;
        }

        [HttpGet]
        public IActionResult Get(int? displayItemId, int? displayId, int? boxId, int? productGroupId, string? searchString, int pageNumber = 1, int pageSize = 10)
        {
            try
            {
                var data = _displayItemService
                        .GetAll(displayItemId, displayId, boxId, productGroupId, searchString, pageNumber, pageSize);
                return Ok(data);
            }
            catch (Exception ex)
            {

                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpPost]
        public IActionResult Add(DisplayItemCreateDTO displayItemCreateDTO)
        {
            try
            {
                var data = _displayItemService.AddDisplayItem(displayItemCreateDTO);
                return CreatedAtAction(nameof(Get), data);
            }
            catch (Exception ex)
            {

                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpPut("{displayItemId}")]
        public IActionResult Update(int displayItemId, DisplayItemUpdateDTO displayItemUpdateDTO)
        {
            try
            {
                var data = _displayItemService.Update(displayItemId, displayItemUpdateDTO);
                return Ok(data);
            }
            catch (Exception ex)
            {

                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpDelete("{displayItemId}")]
        public IActionResult Delete(int displayItemId)
        {
            _displayItemService.Delete(displayItemId);
            return Ok();
        }
    }
}