using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartMenu.API.Ultility;
using SmartMenu.Domain.Models;
using SmartMenu.Domain.Models.DTO;
using SmartMenu.Domain.Models.Enum;
using SmartMenu.Domain.Repository;
using SmartMenu.Service.Interfaces;
using SmartMenu.Service.Services;

namespace SmartMenu.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = SD.Role_BrandManager + "," + SD.Role_StoreManager)]
    public class ProductSizePricesController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IProductSizePriceService _productSizePriceService;

        public ProductSizePricesController(IMapper mapper, IProductSizePriceService productSizePriceService)
        {
            _mapper = mapper;
            _productSizePriceService = productSizePriceService;
        }

        [HttpGet]
        public ActionResult Get(int? productSizePriceId, int? productId, string? searchString, int pageNumber = 1, int pageSize = 10)
        {
            try
            {
                var data = _productSizePriceService.GetAll(productSizePriceId, productId, searchString, pageNumber, pageSize);
                return Ok(data);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            };
        }

        [HttpPost]
        [Authorize(Roles = SD.Role_BrandManager)]
        public async Task<ActionResult> AddAsync(ProductSizePriceCreateDTO productSizePriceCreateDTO)
        {
            try
            {
                var data = await _productSizePriceService.AddAsync(productSizePriceCreateDTO);
                return CreatedAtAction(nameof(Get), data);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpPut("{productSizePriceId}")]
        [Authorize(Roles = SD.Role_BrandManager)]
        public ActionResult Update(int productSizePriceId, ProductSizePriceUpdateDTO productSizePriceUpdateDTO)
        {
            try
            {
                var data = _productSizePriceService.Update(productSizePriceId, productSizePriceUpdateDTO);
                return Ok(data);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpDelete("{productSizePriceId}")]
        [Authorize(Roles = SD.Role_BrandManager)]
        public ActionResult Delete(int productSizePriceId)
        {
            try
            {
                _productSizePriceService.Delete(productSizePriceId);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }
    }
}