using AutoMapper;
using Microsoft.AspNetCore.Mvc;
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
    public class ProductSizePricesController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IProductSizePriceService _productSizePriceService;

        public ProductSizePricesController(IMapper mapper, IUnitOfWork unitOfWork, IProductSizePriceService productSizePriceService)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
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
                return BadRequest(ex.Message);
            };
        }

        [HttpPost]
        public ActionResult Add(ProductSizePriceCreateDTO productSizePriceCreateDTO)
        {
            try
            {
                var data = _productSizePriceService.Add(productSizePriceCreateDTO);
                return CreatedAtAction(nameof(Get), data);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{productSizePriceId}")]
        public ActionResult Update(int productSizePriceId, ProductSizePriceUpdateDTO productSizePriceUpdateDTO)
        {
            try
            {
                var data = _productSizePriceService.Update(productSizePriceId, productSizePriceUpdateDTO);
                return Ok(data);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("{productSizePriceId}")]
        public ActionResult Delete(int productSizePriceId)
        {
            try
            {
                _productSizePriceService.Delete(productSizePriceId);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}