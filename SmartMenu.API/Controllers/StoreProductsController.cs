using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SmartMenu.Domain.Models.DTO;
using SmartMenu.Domain.Models;
using SmartMenu.Domain.Repository;
using SmartMenu.Service.Interfaces;
using SmartMenu.Service.Services;
using Microsoft.AspNetCore.Authorization;
using SmartMenu.API.Ultility;

namespace SmartMenu.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = SD.Role_StoreManager)]
    public class StoreProductsController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IStoreProductService _storeProductService;

        public StoreProductsController(IMapper mapper, IUnitOfWork unitOfWork, IStoreProductService storeProductService)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _storeProductService = storeProductService;
        }

        [HttpGet]
        public ActionResult Get(int? storeProductId, int? productId, int? storeId, string? searchString, int pageSize = 10, int pageNumber = 1)
        {
            try
            {
                var data = _storeProductService.GetAll(storeProductId, storeId, productId, searchString, pageNumber, pageSize);
                return Ok(data);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });

            }
        }

        [HttpGet("productsizeprices")]
        public ActionResult GetWithProductSizePrices(int? storeProductId, int? productId, int? storeId, string? searchString, int pageSize = 10, int pageNumber = 1)
        {
            try
            {
                var data = _storeProductService.GetWithProductSizePrices(storeProductId, storeId, productId, searchString, pageNumber, pageSize);
                return Ok(data);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpPost]
        public ActionResult Add(StoreProductCreateDTO storeProductCreateDTO)
        {
            try
            {
                var data = _storeProductService.Add(storeProductCreateDTO);
                return CreatedAtAction(nameof(Get), data);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpPost("v2")]
        public ActionResult AddV2(StoreProductCreateDTO_V2 storeProductCreateDTO)
        {
            try
            {
                var data = _storeProductService.AddV2(storeProductCreateDTO);
                return CreatedAtAction(nameof(Get), data);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpPut("{storeProductId}")]
        public ActionResult Update(int storeProductId, StoreProductUpdateDTO storeProductUpdateDTO)
        {
            try
            {
                var data = _storeProductService.Update(storeProductId, storeProductUpdateDTO);
                return Ok(data);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpDelete("{storeProductId}")]
        public ActionResult Delete(int storeProductId)
        {
            try
            {
                _storeProductService.Delete(storeProductId);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }
    }
}
