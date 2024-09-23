using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartMenu.API.Ultility;
using SmartMenu.Domain.Models;
using SmartMenu.Domain.Models.DTO;
using SmartMenu.Domain.Repository;
using SmartMenu.Service.Interfaces;
using SmartMenu.Service.Services;

namespace SmartMenu.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = SD.Role_BrandManager + "," + SD.Role_StoreManager)]
    public class ProductGroupItemController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IProductGroupItemService _productGroupItemService;

        public ProductGroupItemController(IMapper mapper, IProductGroupItemService productGroupItemService)
        {
            _mapper = mapper;
            _productGroupItemService = productGroupItemService;
        }

        [HttpGet]
        public IActionResult Get(int? productGroupItemId, int? productGroupId, int? productId, string? searchString, int pageNumber = 1, int pageSize = 10)
        {
            try
            {
                var data = _productGroupItemService.GetAll(productGroupItemId, productGroupId, productId, searchString, pageNumber, pageSize);
                return Ok(data);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            };
        }
        [HttpPost]
        [Authorize(Roles = SD.Role_BrandManager)]
        public IActionResult Add(ProductGroupItemCreateDTO productGroupItemCreateDTO)
        {
            try
            {
                var data = _productGroupItemService.Add(productGroupItemCreateDTO);
                return CreatedAtAction(nameof(Get), data);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        //[HttpPut("{productGroupItemId}")]
        //public IActionResult Update(int productGroupItemId, ProductGroupItemCreateDTO productGroupItemCreateDTO)
        //{
        //    try
        //    {
        //        var data = _productGroupItemService.Update(productGroupItemId, productGroupItemCreateDTO);
        //        return Ok(data);
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(new {error = ex.Message });
        //    }
        //}

        [HttpDelete("{productGroupItemId}")]
        [Authorize(Roles = SD.Role_BrandManager)]
        public IActionResult Delete(int productGroupItemId)
        {
            try
            {
                _productGroupItemService.Delete(productGroupItemId);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }
    }
}
