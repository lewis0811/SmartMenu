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
    public class CategoriesController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly ICategoryService _categoryService;

        public CategoriesController(IMapper mapper, ICategoryService categoryService)
        {
            _mapper = mapper;
            _categoryService = categoryService;
        }

        [HttpGet]
        public ActionResult Get(int? categoryId, int? brandId, string? searchString, int pageNumber = 1, int pageSize = 10)
        {
            try
            {
                var data = _categoryService.GetAll(categoryId, brandId, searchString, pageNumber, pageSize);
                return Ok(data);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            };
        }
        [HttpPost]
        [Authorize(Roles = SD.Role_BrandManager)]
        public ActionResult Add(CategoryCreateDTO categoryCreateDTO)
        {
            try
            {
                var data = _categoryService.Add(categoryCreateDTO);
                return CreatedAtAction(nameof(Get), data);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpPut("{categoryId}")]
        [Authorize(Roles = SD.Role_BrandManager)]
        public ActionResult Update(int categoryId, CategoryCreateDTO categoryCreateDTO)
        {
            try
            {
                var data = _categoryService.Update(categoryId, categoryCreateDTO);
                return Ok(data);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpDelete("{categoryId}")]
        [Authorize(Roles = SD.Role_BrandManager)]
        public ActionResult Delete(int categoryId)
        {
            try
            {
                _categoryService.Delete(categoryId);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }
    }
}
