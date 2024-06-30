using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SmartMenu.Domain.Models.DTO;
using SmartMenu.Domain.Models;
using SmartMenu.Domain.Repository;
using SmartMenu.Service.Interfaces;
using SmartMenu.Service.Services;

namespace SmartMenu.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
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
        public ActionResult Get(int? storeProductId, int? productId, int? storeId,string? searchString, int pageSize = 1, int pageNumber = 10)
        {
            try
            {
                var data = _storeProductService.GetAll(storeProductId, storeId, productId, searchString, pageNumber, pageSize);
                return Ok(data);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
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
                return BadRequest(ex.Message);
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
                return BadRequest(ex.Message);
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
                return BadRequest(ex.Message);
            }
        }
    }
}
