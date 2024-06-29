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
    public class MenusController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IMenuService _menuService;

        public MenusController(IUnitOfWork unitOfWork, IMapper mapper, IMenuService menuService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _menuService = menuService;
        }
        [HttpGet]
        public ActionResult Get(int? menuId, int? brandId, string? searchString, int pageNumber = 1, int pageSize = 10)
        {
            try
            {
                var data = _menuService.GetAll(menuId, brandId, searchString, pageNumber, pageSize);
                return Ok(data);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("ProductGroup")]
        public ActionResult GetMenuProductGroup(int? menuId, int? brandId, string? searchString, int pageNumber = 1, int pageSize = 10)
        {
            try
            {
                var data = _menuService.GetMenuWithProductGroup(menuId, brandId, searchString, pageNumber, pageSize);
                return Ok(data);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpPost]
        public ActionResult Add(MenuCreateDTO menuCreateDTO)
        {
            try
            {
                var data = _menuService.Add(menuCreateDTO);
                return CreatedAtAction(nameof(Get), data);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{menuId}")]
        public ActionResult Update(int menuId, MenuUpdateDTO menuUpdateDTO)
        {
            try
            {
                var data = _menuService.Update(menuId, menuUpdateDTO);
                return Ok(data);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("{menuId}")]
        public ActionResult Delete(int menuId)
        {
            try
            {
                _menuService.Delete(menuId);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
