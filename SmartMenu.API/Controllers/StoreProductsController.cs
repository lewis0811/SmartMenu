using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SmartMenu.Domain.Models.DTO;
using SmartMenu.Domain.Models;
using SmartMenu.Domain.Repository;

namespace SmartMenu.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StoreProductsController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;

        public StoreProductsController(IMapper mapper, IUnitOfWork unitOfWork)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        public IActionResult Get(int? storeProductId, string? searchString, int pageSize = 1, int pageNumber = 10)
        {
            var data = _unitOfWork.StoreProductRepository.GetStoreProducts(storeProductId, searchString, pageSize, pageNumber);
            data ??= Enumerable.Empty<StoreProduct>();

            return Ok(data);
        }

        [HttpPost]
        public IActionResult Add(StoreProductCreateDTO storeProductCreateDTO)
        {
            var data = _mapper.Map<StoreProduct>(storeProductCreateDTO);
            _unitOfWork.StoreProductRepository.Add(data);
            _unitOfWork.Save();
            return CreatedAtAction(nameof(Get), data);
        }

        [HttpPut("{storeProductId}")]
        public IActionResult Update(int storeProductId, StoreProductUpdateDTO storeProductUpdateDTO)
        {
            var data = _unitOfWork.StoreProductRepository.Find(c => c.StoreProductId == storeProductId && c.IsDeleted == false).FirstOrDefault();
            if (data == null) return BadRequest("StoreProduct not found or deleted");

            _mapper.Map(storeProductUpdateDTO, data);
            _unitOfWork.StoreProductRepository.Update(data);
            _unitOfWork.Save();
            return Ok(data);
        }

        [HttpDelete("{storeProductId}")]
        public IActionResult Delete(int storeProductId)
        {
            var data = _unitOfWork.StoreProductRepository.Find(c => c.StoreProductId == storeProductId && c.IsDeleted == false).FirstOrDefault();
            if (data == null) return BadRequest("StoreProduct not found or deleted");

            data.IsDeleted = true;
            _unitOfWork.StoreProductRepository.Update(data);
            _unitOfWork.Save();
            return Ok();
        }
    }
}
