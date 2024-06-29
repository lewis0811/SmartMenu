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
    public class CategoriesController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ICategoryService _categoryService;

        public CategoriesController(IUnitOfWork unitOfWork, IMapper mapper, ICategoryService categoryService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _categoryService = categoryService;
        }

        [HttpGet]
        public ActionResult Get(int? categoryId, string? searchString, int pageNumber = 1, int pageSize = 10)
        {
            try
            {
                var data = _categoryService.GetAll(categoryId, searchString, pageNumber, pageSize);
                return Ok(data);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            };
        }
        [HttpPost]
        public ActionResult Add(CategoryCreateDTO categoryCreateDTO)
        {
            try
            {
                var data = _categoryService.Add(categoryCreateDTO);
                return CreatedAtAction(nameof(Get), data);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{categoryId}")]
        public ActionResult Update(int categoryId, CategoryCreateDTO categoryCreateDTO)
        {
            try
            {
                var data = _categoryService.Update(categoryId, categoryCreateDTO);
                return Ok(data);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("{categoryId}")]
        public ActionResult Delete(int categoryId)
        {
            try
            {
                _categoryService.Delete(categoryId);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
