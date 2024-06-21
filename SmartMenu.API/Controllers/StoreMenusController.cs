using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using SmartMenu.Domain.Models;
using SmartMenu.Domain.Models.DTO;
using SmartMenu.Domain.Repository;

namespace SmartMenu.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StoreMenusController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public StoreMenusController(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        [HttpGet]
        public IActionResult Get(int? storeMenuId, int? storeId, int? menuId, string? searchString, int pageNumber = 1, int pageSize = 10)
        {
            var data = _unitOfWork.StoreMenuRepository.GetAll(storeMenuId, storeId, menuId, searchString, pageNumber, pageSize);
            return Ok(data);
        }
        [HttpPost]
        public IActionResult Add(StoreMenuCreateDTO storeMenuCreateDTO)
        {

            var data = _mapper.Map<StoreMenu>(storeMenuCreateDTO);
            _unitOfWork.StoreMenuRepository.Add(data);
            _unitOfWork.Save();
            return CreatedAtAction(nameof(Get), new { data });
        }

        [HttpPut("{storeMenuId}")]
        public IActionResult Update(int storeMenuId, StoreMenuCreateDTO storeMenuCreateDTO)
        {
            var data = _unitOfWork.StoreMenuRepository.Find(c => c.StoreMenuID == storeMenuId && c.IsDeleted == false).FirstOrDefault();
            if (data == null) return BadRequest("StoreMenu not found or deleted");
            _mapper.Map(storeMenuCreateDTO, data);
            _unitOfWork.StoreMenuRepository.Update(data);
            _unitOfWork.Save();
            return Ok(data);
        }

        [HttpDelete("{storeMenuId}")]
        public IActionResult Delete(int storeMenuId)
        {
            var data = _unitOfWork.StoreMenuRepository.Find(c => c.StoreMenuID == storeMenuId && c.IsDeleted == false).FirstOrDefault();
            if (data == null) return BadRequest("StoreMenu not found or deleted");

            data.IsDeleted = true;
            _unitOfWork.StoreMenuRepository.Update(data);
            _unitOfWork.Save();
            return Ok();
        }
    }
}
