using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using SmartMenu.Domain.Models;
using SmartMenu.Domain.Models.DTO;
using SmartMenu.Domain.Repository;
using SmartMenu.Service.Interfaces;
using SmartMenu.Service.Services;

namespace SmartMenu.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductGroupItemController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IProductGroupItemService _productGroupItemService;

        public ProductGroupItemController(IUnitOfWork unitOfWork, IMapper mapper, IProductGroupItemService productGroupItemService)
        {
            _unitOfWork = unitOfWork;
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
                return BadRequest(new {error = ex.Message });
            };
        }
        [HttpPost]
        public IActionResult Add(ProductGroupItemCreateDTO productGroupItemCreateDTO)
        {
            try
            {
                var data = _productGroupItemService.Add(productGroupItemCreateDTO);
                return CreatedAtAction(nameof(Get), data);
            }
            catch (Exception ex)
            {
                return BadRequest(new {error = ex.Message });
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
        public IActionResult Delete(int productGroupItemId)
        {
            try
            {
                _productGroupItemService.Delete(productGroupItemId);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(new {error = ex.Message });
            }
        }
    }
}
