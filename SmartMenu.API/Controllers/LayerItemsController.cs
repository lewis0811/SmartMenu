using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using SmartMenu.Domain.Models;
using SmartMenu.Domain.Models.DTO;
using SmartMenu.Domain.Repository;
using SmartMenu.Service.Interfaces;

namespace SmartMenu.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LayerItemsController : ControllerBase
    {
        private readonly IlayerItemService _layerItemService;

        public LayerItemsController(IlayerItemService layerItemService)
        {
            _layerItemService = layerItemService;
        }

        [HttpGet]
        public IActionResult Get(int? layerItemId, int? layerId, string? searchString, int pageNumber = 1, int pageSize = 10)
        {
            try
            {
                var data = _layerItemService.GetAll(layerItemId, layerId, searchString, pageNumber, pageSize);
                return Ok(data);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        public IActionResult Add(LayerItemCreateDTO layerItemCreateDTO)
        {
            try
            {
                var data = _layerItemService.AddLayerItem(layerItemCreateDTO);
                return CreatedAtAction(nameof(Get), data);
            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{layerItemId}")]
        public IActionResult Update(int layerItemId, LayerItemUpdateDTO layerItemUpdateDTO)
        {
            try
            {
                var data = _layerItemService.Update(layerItemId, layerItemUpdateDTO);
                return Ok(data);
            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("{layerItemId}")]
        public IActionResult Delete(int layerItemId)
        {
            try
            {
                _layerItemService.Delete(layerItemId);
                return Ok();
            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }
        }
    }
}