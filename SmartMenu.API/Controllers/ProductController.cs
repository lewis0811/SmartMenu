using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using SmartMenu.Domain.Models;
using SmartMenu.Domain.Models.DTO;
using SmartMenu.Domain.Repository;

namespace SmartMenu.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public ProductController(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        [HttpGet]
        public IActionResult Get(int? storeId, int? categoryId, int? brandId, string? searchString, int pageNumber = 1, int pageSize = 10)
        {
            var data = _unitOfWork.ProductRepository.GetAll(storeId, categoryId, brandId, searchString, pageNumber, pageSize);
            return Ok(data);
        }
        [HttpPost]
        public IActionResult Add(ProductCreateDTO productCreateDTO)
        {
            var data = _mapper.Map<Product>(productCreateDTO);
            _unitOfWork.ProductRepository.Add(data);
            _unitOfWork.Save();
            return CreatedAtAction(nameof(Get), new { data });
        }

        [HttpPut]
        public IActionResult Update(int productId, ProductCreateDTO productCreateDTO)
        {
            var data = _unitOfWork.ProductRepository.Find(c => c.ProductID == productId).FirstOrDefault();
            if (data == null) return NotFound();

            _mapper.Map(productCreateDTO, data);

            _unitOfWork.ProductRepository.Update(data);
            _unitOfWork.Save();
            return Ok(data);
        }

        [HttpDelete]
        public IActionResult Delete(int productId)
        {
            var data = _unitOfWork.ProductRepository.Find(c => c.ProductID == productId).FirstOrDefault();
            if (data == null) return NotFound();

            data.IsDeleted = true;
            _unitOfWork.ProductRepository.Update(data);
            _unitOfWork.Save();
            return Ok();
        }
    }
}
