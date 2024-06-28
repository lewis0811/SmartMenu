using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using SmartMenu.Domain.Models;
using SmartMenu.Domain.Models.DTO;
using SmartMenu.Domain.Repository;

namespace SmartMenu.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public ProductsController(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        [HttpGet]
        public IActionResult Get(int? storeId, int? categoryId, string? searchString, int pageNumber = 1, int pageSize = 10)
        {
            var data = _unitOfWork.ProductRepository.GetAll(storeId, categoryId, searchString, pageNumber, pageSize);
            return Ok(data);
        }
        [HttpPost]
        public IActionResult Add(ProductCreateDTO productCreateDTO)
        {
            var category = _unitOfWork.CategoryRepository.Find(c => c.CategoryId == productCreateDTO.CategoryId && c.IsDeleted == false).FirstOrDefault();
            if (category == null) return BadRequest("Category not found or deleted");

            var data = _mapper.Map<Product>(productCreateDTO);
            _unitOfWork.ProductRepository.Add(data);
            _unitOfWork.Save();
            return CreatedAtAction(nameof(Get), new { data });
        }

        [HttpPut("{productId}")]
        public IActionResult Update(int productId, ProductCreateDTO productCreateDTO)
        {
            var data = _unitOfWork.ProductRepository.Find(c => c.ProductId == productId && c.IsDeleted == false).FirstOrDefault();
            if (data == null) return BadRequest("Product not found or deleted");

            _mapper.Map(productCreateDTO, data);

            _unitOfWork.ProductRepository.Update(data);
            _unitOfWork.Save();
            return Ok(data);
        }

        [HttpDelete("{productId}")]
        public IActionResult Delete(int productId)
        {
            var data = _unitOfWork.ProductRepository.Find(c => c.ProductId == productId && c.IsDeleted == false).FirstOrDefault();
            if (data == null) return BadRequest("Product not found or deleted");

            data.IsDeleted = true;
            _unitOfWork.ProductRepository.Update(data);
            _unitOfWork.Save();
            return Ok();
        }
    }
}
