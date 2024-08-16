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
    public class ProductsController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IProductService _productService;

        public ProductsController(IUnitOfWork unitOfWork, IMapper mapper, IProductService productService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _productService = productService;
        }

        [HttpGet]
        public ActionResult Get(int? productId, int? categoryId, string? searchString, int pageNumber = 1, int pageSize = 10)
        {
            try
            {
                var data = _productService.GetAll(productId, categoryId,  searchString, pageNumber, pageSize);
                return Ok(data);
            }
            catch (Exception ex)
            {
                return BadRequest(new {error = ex.Message });
            };
        }
        [HttpPost]
        public ActionResult Add(ProductCreateDTO productCreateDTO)
        {
            try
            {
                var data = _productService.Add(productCreateDTO);
                return CreatedAtAction(nameof(Get), data);
            }
            catch (Exception ex)
            {
                return BadRequest(new {error = ex.Message });
            }
        }

        [HttpPut("{productId}")]
        public ActionResult Update(int productId, ProductUpdateDTO productCreateDTO)
        {
            try
            {
                var data = _productService.Update(productId, productCreateDTO);
                return Ok(data);
            }
            catch (Exception ex)
            {
                return BadRequest(new {error = ex.Message });
            }
        }

        [HttpDelete("{productId}")]
        public ActionResult Delete(int productId)
        {
            try
            {
                _productService.Delete(productId);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(new {error = ex.Message });
            }
        }
    }
}
