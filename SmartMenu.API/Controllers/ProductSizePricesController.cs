using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using SmartMenu.Domain.Models;
using SmartMenu.Domain.Models.DTO;
using SmartMenu.Domain.Repository;

namespace SmartMenu.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductSizePricesController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;

        public ProductSizePricesController(IMapper mapper, IUnitOfWork unitOfWork)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        public IActionResult Get(int? productSizePriceId, int? productId, string? searchString, int pageNumber = 1, int pageSize = 10)
        {
            var data = _unitOfWork.ProductSizePriceRepository.GetAll(productSizePriceId, productId, productId, searchString, pageNumber, pageSize);
            if (data == null) data ??= Enumerable.Empty<ProductSizePrice>();

            return Ok(data);
        }

        [HttpPost]
        public IActionResult Add(ProductSizePriceCreateDTO productSizePriceCreateDTO)
        {
            var data = _mapper.Map<ProductSizePrice>(productSizePriceCreateDTO);
            _unitOfWork.ProductSizePriceRepository.Add(data);
            _unitOfWork.Save();
            return CreatedAtAction(nameof(Get), data);
        }

        [HttpPut("{productSizePriceId}")]
        public IActionResult Update(int productSizePriceId, ProductSizePriceUpdateDTO productSizePriceUpdateDTO)
        {
            var data = _unitOfWork.ProductSizePriceRepository.Find(c => c.ProductSizePriceId == productSizePriceId && c.IsDeleted == false).FirstOrDefault();
            if (data == null) return BadRequest("ProductSizePrice not found or deleted");

            _mapper.Map(productSizePriceUpdateDTO, data);
            _unitOfWork.ProductSizePriceRepository.Update(data);
            _unitOfWork.Save();
            return Ok(data);
        }

        [HttpDelete("{productSizePriceId}")]
        public IActionResult Delete(int productSizePriceId)
        {
            var data = _unitOfWork.ProductSizePriceRepository.Find(c => c.ProductSizePriceId == productSizePriceId && c.IsDeleted == false).FirstOrDefault();
            if (data == null) return BadRequest("ProductSizePrice not found or deleted");

            data.IsDeleted = true;
            _unitOfWork.ProductSizePriceRepository.Update(data);
            _unitOfWork.Save();
            return Ok();
        }
    }
}