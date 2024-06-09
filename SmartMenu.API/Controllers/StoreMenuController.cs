using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using SmartMenu.Domain.Models;
using SmartMenu.Domain.Models.DTO;
using SmartMenu.Domain.Repository;

namespace SmartMenu.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StoreMenuController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public StoreMenuController(IUnitOfWork unitOfWork, IMapper mapper)
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

        [HttpPut]
        public IActionResult Update(int storeMenuId, StoreMenuCreateDTO storeMenuCreateDTO)
        {
            var data = _unitOfWork.StoreMenuRepository.Find(c => c.StoreMenuID == storeMenuId).FirstOrDefault();
            if (data == null) return NotFound();
            _mapper.Map(storeMenuCreateDTO, data);
            _unitOfWork.StoreMenuRepository.Update(data);
            _unitOfWork.Save();
            return Ok(data);
        }

        [HttpDelete]
        public IActionResult Delete(int storeMenuId)
        {
            var data = _unitOfWork.StoreMenuRepository.Find(c => c.StoreMenuID == storeMenuId).FirstOrDefault();
            if (data == null) return NotFound();

            data.IsDeleted = true;
            _unitOfWork.StoreMenuRepository.Update(data);
            _unitOfWork.Save();
            return Ok();
        }
    }
}
