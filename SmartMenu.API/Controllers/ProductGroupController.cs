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
    public class ProductGroupController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IProductGroupService _productGroupService;

        public ProductGroupController(IMapper mapper, IProductGroupService productGroupService)
        {
            _mapper = mapper;
            _productGroupService = productGroupService;
        }

        [HttpGet]
        public ActionResult Get(int? productGroupId, int? menuId, int? collectionId, string? searchString, int pageNumber = 1, int pageSize = 10)
        {
            try
            {
                var data = _productGroupService.GetAll(productGroupId, menuId, collectionId, searchString, pageNumber, pageSize);
                return Ok(data);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpGet("GroupItem")]
        public ActionResult GetProductGroupWithGroupItem(int? productGroupId, int? menuId, int? collectionId, string? searchString, int pageNumber = 1, int pageSize = 10)
        {
            try
            {
                var data = _productGroupService.GetProductGroupWithGroupItem(productGroupId, menuId, collectionId, searchString, pageNumber, pageSize);
                return Ok(data);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpPost]
        [Authorize(Roles = SD.Role_BrandManager)]
        public ActionResult Add(ProductGroupCreateDTO productCreateDTO)
        {
            try
            {
                var data = _productGroupService.Add(productCreateDTO);
                return CreatedAtAction(nameof(Get), data);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpPut("{productGroupId}")]
        [Authorize(Roles = SD.Role_BrandManager)]
        public ActionResult Update(int productGroupId, ProductGroupUpdateDTO productGroupUpdateDTO)
        {
            try
            {
                var data = _productGroupService.Update(productGroupId, productGroupUpdateDTO);
                return Ok(data);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpDelete("{productGroupId}")]
        [Authorize(Roles = SD.Role_BrandManager)]
        public ActionResult Delete(int productGroupId)
        {
            try
            {
                _productGroupService.Delete(productGroupId);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }
    }
}