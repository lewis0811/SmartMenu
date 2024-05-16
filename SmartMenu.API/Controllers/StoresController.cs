using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartMenu.Domain.Models;
using SmartMenu.Domain.Models.DTO;
using SmartMenu.Domain.Repository;

namespace SmartMenu.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StoresController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public StoresController(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        [HttpGet]
        public IActionResult Get(int? storeId, int? brandId, string? searchString, int pageNumber = 1, int pageSize = 10)
        {
            var data = _unitOfWork.StoreRepository.GetAll(storeId, brandId, searchString, pageNumber, pageSize);
            return Ok(data);
        }

        [HttpGet("StoreMenus")]
        public IActionResult GetStoreMenus(int? storeId, int? brandId, string? searchString, int pageNumber = 1, int pageSize = 10)
        {
            var data = _unitOfWork.StoreRepository.GetStoreWithMenus(storeId, brandId, searchString, pageNumber, pageSize);
            return Ok(data);
        }

        [HttpGet("StoreCollections")]
        public IActionResult GetStoreCollection(int? storeId, int? brandId, string? searchString, int pageNumber = 1, int pageSize = 10)
        {
            var data = _unitOfWork.StoreRepository.GetStoreWithCollections(storeId, brandId, searchString, pageNumber, pageSize);
            return Ok(data);
        }

        [HttpPost]
        public IActionResult Add(StoreCreateDTO storeCreateDTO)
        {
            var data = _mapper.Map<Store>(storeCreateDTO);
            _unitOfWork.StoreRepository.Add(data);
            _unitOfWork.Save();
            return CreatedAtAction(nameof(Get), new {data});
        }

        [HttpPut]
        public IActionResult Update(int storeId, StoreUpdateDTO storeUpdateDTO)
        {
            var data = _unitOfWork.StoreRepository.Find(c => c.StoreID == storeId).FirstOrDefault();
            if (data == null) return NotFound();

            _mapper.Map(storeUpdateDTO, data);
            
            _unitOfWork.StoreRepository.Update(data);
            _unitOfWork.Save();
            return Ok(data);
        }

        [HttpDelete]
        public IActionResult Delete(int storeId)
        {
            var data = _unitOfWork.StoreRepository.Find(c => c.StoreID == storeId).FirstOrDefault();
            if (data == null) return NotFound();

            data.IsDeleted = true;
            _unitOfWork.StoreRepository.Update(data);
            _unitOfWork.Save();
            return Ok();
        }
    }
}