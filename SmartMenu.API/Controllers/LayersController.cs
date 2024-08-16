using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using SmartMenu.Domain.Models.DTO;
using SmartMenu.Domain.Repository;
using SmartMenu.Service.Interfaces;

namespace SmartMenu.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LayersController : ControllerBase
    {
        private readonly ILayerService _layerService;

        public LayersController( ILayerService layerService)
        {
            _layerService = layerService;
        }

        [HttpGet]
        public IActionResult Get(int? layerId, int? templateId, string? searchString, int pageNumber = 1, int pageSize = 10)
        {
            try
            {
                var data = _layerService.GetAll(layerId, templateId, searchString, pageNumber, pageSize);
                return Ok(data);
            }
            catch (Exception ex)
            {
                return BadRequest(new {error = ex.Message });
            }
        }

        [HttpGet("LayerItemsBoxes")]
        public IActionResult GetLayerItemsAndBoxes(int? layerId, int? templateId, string? searchString, int pageNumber = 1, int pageSize = 10)
        {
            try
            {
                var data = _layerService.GetAllWithLayerItemsAndBoxes(layerId, templateId, searchString, pageNumber, pageSize);
                return Ok(data);
            }
            catch (Exception ex)
            {
                return BadRequest(new {error = ex.Message });
            }
        }

        [HttpGet("LayerItems")]
        public IActionResult GetLayerItems(int? layerId, int? templateId, string? searchString, int pageNumber = 1, int pageSize = 10)
        {
            try
            {
                var data = _layerService.GetAllWithLayerItems(layerId, templateId, searchString, pageNumber, pageSize);
                return Ok(data);
            }
            catch (Exception ex)
            {
                return BadRequest(new {error = ex.Message });
            }
        }

        [HttpGet("Boxes")]
        public IActionResult GetWithBoxes(int? layerId, int? templateId, string? searchString, int pageNumber = 1, int pageSize = 10)
        {
            try
            {
                var data = _layerService.GetAllWithBoxes(layerId, templateId, searchString, pageNumber, pageSize);
                return Ok(data);
            }
            catch (Exception ex)
            {
                return BadRequest(new {error = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> AddAsync(LayerCreateDTO layerCreateDTO)
        {
            try
            {
                var data = await _layerService.AddLayerAsync(layerCreateDTO);
                return CreatedAtAction(nameof(Get), data);
            }
            catch (Exception ex)
            {
                return BadRequest(new {error = ex.Message });
            };
        }

        [HttpPut("{layerId}")]
        public IActionResult Update(int layerId, LayerUpdateDTO layerUpdateDTO)
        {
            try
            {
                var data = _layerService.Update(layerId, layerUpdateDTO);
                return Ok(data);
            }
            catch (Exception ex)
            {
                return BadRequest(new {error = ex.Message });
            }
        }

        [HttpDelete("{layerId}")]
        public IActionResult Delete(int layerId)
        {
            try
            {
                _layerService.Delete(layerId);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(new {error = ex.Message });
            }
        }
    }
}