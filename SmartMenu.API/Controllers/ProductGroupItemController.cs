using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using SmartMenu.Domain.Models;
using SmartMenu.Domain.Models.DTO;
using SmartMenu.Domain.Repository;

namespace SmartMenu.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductGroupItemController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public ProductGroupItemController(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        [HttpGet]
        public IActionResult Get(int? productGroupItemId, int? productGroupId, int? productId, string? searchString, int pageNumber = 1, int pageSize = 10)
        {
            var data = _unitOfWork.ProductGroupItemRepository.GetAll(productGroupItemId, productGroupId, productId, searchString, pageNumber, pageSize);
            return Ok(data);
        }
        [HttpPost]
        public IActionResult Add(ProductGroupItemCreateDTO productGroupItemCreateDTO)
        {
            var data = _mapper.Map<ProductGroupItem>(productGroupItemCreateDTO);
            _unitOfWork.ProductGroupItemRepository.Add(data);
            _unitOfWork.Save();
            return CreatedAtAction(nameof(Get), new { data });
        }

        [HttpPut("{productGroupItemId}")]
        public IActionResult Update(int productGroupItemId, ProductGroupItemCreateDTO productGroupItemCreateDTO)
        {
            var data = _unitOfWork.ProductGroupItemRepository.Find(c => c.ProductGroupItemId == productGroupItemId).FirstOrDefault();
            if (data == null) return NotFound();
            _mapper.Map(productGroupItemCreateDTO, data);
            _unitOfWork.ProductGroupItemRepository.Update(data);
            _unitOfWork.Save();
            return Ok(data);
        }

        [HttpDelete("{productGroupItemId}")]
        public IActionResult Delete(int productGroupItemId)
        {
            var data = _unitOfWork.ProductGroupItemRepository.Find(c => c.ProductGroupItemId == productGroupItemId).FirstOrDefault();
            if (data == null) return NotFound();

            data.IsDeleted = true;
            _unitOfWork.ProductGroupItemRepository.Update(data);
            _unitOfWork.Save();
            return Ok();
        }
    }
}
