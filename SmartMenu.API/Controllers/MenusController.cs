using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using SmartMenu.Domain.Models;
using SmartMenu.Domain.Models.DTO;
using SmartMenu.Domain.Repository;

namespace SmartMenu.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MenusController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public MenusController(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        [HttpGet]
        public IActionResult Get(int? menuId, int? brandId, string? searchString, int pageNumber = 1, int pageSize = 10)
        {
            var data = _unitOfWork.MenuRepository.GetAll(menuId, brandId, searchString, pageNumber, pageSize);
            return Ok(data);
        }

        [HttpGet("ProductGroup")]
        public IActionResult GetMenuProductGroup(int? menuId, int? brandId, string? searchString, int pageNumber = 1, int pageSize = 10)
        {
            var data = _unitOfWork.MenuRepository.GetMenuWithProductGroup(menuId, brandId, searchString, pageNumber, pageSize);
            return Ok(data);
        }
        [HttpPost]
        public IActionResult Add(MenuCreateDTO menuCreateDTO)
        {
            var data = _mapper.Map<Menu>(menuCreateDTO);
            _unitOfWork.MenuRepository.Add(data);
            _unitOfWork.Save();
            return CreatedAtAction(nameof(Get), new { data });
        }

        [HttpPut("{menuId}")]
        public IActionResult Update(int menuId, MenuUpdateDTO menuUpdateDTO)
        {
            var data = _unitOfWork.MenuRepository.Find(c => c.MenuId == menuId && c.IsDeleted == false).FirstOrDefault();
            if (data == null) return BadRequest("Menu not found or deleted");

            _mapper.Map(menuUpdateDTO, data);

            _unitOfWork.MenuRepository.Update(data);
            _unitOfWork.Save();
            return Ok(data);
        }

        [HttpDelete("{menuId}")]
        public IActionResult Delete(int menuId)
        {
            var data = _unitOfWork.MenuRepository.Find(c => c.MenuId == menuId && c.IsDeleted == false).FirstOrDefault();
            if (data == null) return BadRequest("Menu not found or deleted");

            data.IsDeleted = true;
            _unitOfWork.MenuRepository.Update(data);
            _unitOfWork.Save();
            return Ok();
        }
    }
}
