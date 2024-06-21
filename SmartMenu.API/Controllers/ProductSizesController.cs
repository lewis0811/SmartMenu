using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using SmartMenu.Domain.Models;
using SmartMenu.Domain.Models.DTO;
using SmartMenu.Domain.Repository;

namespace SmartMenu.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductSizesController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;

        public ProductSizesController(IMapper mapper, IUnitOfWork unitOfWork)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        public IActionResult Get(int? productSizeId, string? searchString, int pageNumber = 1, int pageSize = 10)
        {
            var data = _unitOfWork.ProductSizeRepository.GetAll(productSizeId, searchString, pageNumber, pageSize);
            data ??= Enumerable.Empty<ProductSize>();

            return Ok(data);
        }

        [HttpPost]
        public IActionResult Add(ProductSizeCreateDTO productSizeCreateDTO)
        {
            var data = _mapper.Map<ProductSize>(productSizeCreateDTO);
            _unitOfWork.ProductSizeRepository.Add(data);
            _unitOfWork.Save();
            return CreatedAtAction(nameof(Get), data);
        }

        [HttpPut("{productSizeId}")]
        public IActionResult Update(int productSizeId, ProductCreateDTO productSizeUpdateDTO)
        {
            var data = _unitOfWork.ProductSizeRepository.Find(c => c.ProductSizeId == productSizeId && c.IsDeleted == false).FirstOrDefault();
            if (data == null) return BadRequest("ProductSize not found or deleted");

            _mapper.Map(productSizeUpdateDTO, data);
            _unitOfWork.ProductSizeRepository.Update(data);
            _unitOfWork.Save();
            return Ok(data);
        }

        [HttpDelete("{productSizeId}")]
        public IActionResult Delete(int productSizeId)
        {
            var data = _unitOfWork.ProductSizeRepository.Find(c => c.ProductSizeId == productSizeId && c.IsDeleted == false).FirstOrDefault();
            if (data == null) return BadRequest("ProductSize not found or deleted");

            data.IsDeleted = true;
            _unitOfWork.ProductSizeRepository.Update(data);
            _unitOfWork.Save();
            return Ok();
        }
    }
}