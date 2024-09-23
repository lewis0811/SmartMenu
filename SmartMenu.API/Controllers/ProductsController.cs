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
    public class ProductsController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IProductService _productService;

        public ProductsController(IMapper mapper, IProductService productService)
        {
            _mapper = mapper;
            _productService = productService;
        }

        [HttpGet]
        public ActionResult Get(int? productId, int? categoryId, string? searchString, int pageNumber = 1, int pageSize = 10)
        {
            try
            {
                var data = _productService.GetAll(productId, categoryId, searchString, pageNumber, pageSize);
                return Ok(data);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            };
        }

        [HttpGet("{brandId}")]
        public ActionResult GetByBrand(int brandId, string? searchString, int pageNumber = 1, int pageSize = 10)
        {
            try
            {
                var data = _productService.GetByBrand(brandId, searchString, pageNumber, pageSize);
                return Ok(data);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            };
        }

        [HttpGet("menu-collection")]
        public IActionResult GetProductByMenu(int? menuId, int? collectionId)
        {
            try
            {
                if (menuId == null && collectionId == null) throw new Exception("Please input menuId or collectionId");

                var data = _productService.GetProductByMenuOrCollection(menuId, collectionId);
                return Ok(data);
            }
            catch (Exception ex)
            {

                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpPost]
        [Authorize(Roles = SD.Role_BrandManager)]
        public ActionResult Add(ProductCreateDTO productCreateDTO)
        {
            try
            {
                var data = _productService.Add(productCreateDTO);
                return CreatedAtAction(nameof(Get), data);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpPut("{productId}")]
        [Authorize(Roles = SD.Role_BrandManager)]
        public ActionResult Update(int productId, ProductUpdateDTO productCreateDTO)
        {
            try
            {
                var data = _productService.Update(productId, productCreateDTO);
                return Ok(data);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpDelete("{productId}")]
        [Authorize(Roles = SD.Role_BrandManager)]
        public ActionResult Delete(int productId)
        {
            try
            {
                _productService.Delete(productId);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }
    }
}
