using Microsoft.AspNetCore.Mvc;
using SmartMenu.Domain.Models.DTO;
using SmartMenu.Service.Interfaces;

namespace SmartMenu.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DisplaysController : ControllerBase
    {
        private readonly IDisplayService _displayService;

        public DisplaysController(IDisplayService displayService)
        {
            _displayService = displayService;
        }

        [HttpGet]
        public IActionResult Get(int? displayId, int? menuId, int? collectionId, string? searchString, int pageNumber = 1, int pageSize = 10)
        {
            try
            {
                var data = _displayService.GetAll(displayId, menuId, collectionId, searchString, pageNumber, pageSize);
                return Ok(data);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("DisplayItems")]
        public IActionResult GetWithItems(int? displayId, int? menuId, int? collectionId, string? searchString, int pageNumber = 1, int pageSize = 10)
        {
            try
            {
                var data = _displayService.GetWithItems(displayId, menuId, collectionId, searchString, pageNumber, pageSize);
                return Ok(data);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("{displayId}/image")]
        public IActionResult Get(int displayId)
        {
            try
            {
                var data = _displayService.GetImageById(displayId);
                if (data == null) return BadRequest("Image fail to create");

                byte[] b = System.IO.File.ReadAllBytes(data);
                return File(b, "image/jpeg");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        public IActionResult Add(DisplayCreateDTO displayCreateDTO)
        {
            try
            {
                var data = _displayService.AddDisplay(displayCreateDTO);
                return CreatedAtAction(nameof(Get), data);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        //[HttpPost("V2")]
        //[ProducesResponseType(StatusCodes.Status201Created)]
        //public IActionResult AddV2(DisplayCreateDTO displayCreateDTO)
        //{
        //    try
        //    {
        //        var data = _displayService.AddDisplayV2(displayCreateDTO);
        //        return CreatedAtAction(nameof(Get), new { displayId = data.DisplayId });
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(ex.Message);
        //    }
        //}

        [HttpPost("V3")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public IActionResult AddV3(DisplayCreateDTO displayCreateDTO)
        {
            try
            {
                var data = _displayService.AddDisplayV3(displayCreateDTO);
                return CreatedAtAction(nameof(Get), new { displayId = data.DisplayId });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{displayId}")]
        public IActionResult Update(int displayId, DisplayUpdateDTO displayUpdateDTO)
        {
            try
            {
                var data = _displayService.Update(displayId, displayUpdateDTO);
                return Ok(data);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{displayId}/image")]
        public IActionResult UpdateContainImage(int displayId, DisplayUpdateDTO displayUpdateDTO)
        {
            try
            {
                var data = _displayService.UpdateContainImage(displayId, displayUpdateDTO);
                if (data == null) return BadRequest("Image fail to create");

                byte[] b = System.IO.File.ReadAllBytes(data);
                return File(b, "image/jpeg");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("{displayId}")]
        public IActionResult Delete(int displayId)
        {
            try
            {
                _displayService.Delete(displayId);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}