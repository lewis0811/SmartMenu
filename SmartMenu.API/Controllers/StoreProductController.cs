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
    public class StoreProductController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;

        public StoreProductController(IMapper mapper, IUnitOfWork unitOfWork)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        public IActionResult Get(int? storeProductId, string? searchString, int pageSize = 1, int pageNumber = 10)
        {
            var data = _unitOfWork.StoreProductRepository.GetStoreProducts(storeProductId, searchString, pageSize, pageNumber).ToList();
            if (data.Count == 0) return NotFound();

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
    }
}
