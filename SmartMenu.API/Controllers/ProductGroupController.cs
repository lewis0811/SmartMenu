using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using SmartMenu.Domain.Models;
using SmartMenu.Domain.Models.DTO;
using SmartMenu.Domain.Repository;

namespace SmartMenu.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductGroupController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public ProductGroupController(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        [HttpGet]
        public IActionResult Get(int? productGroupId, int? menuId, int? collectionId, string? searchString, int pageNumber = 1, int pageSize = 10)
        {
            var data = _unitOfWork.ProductGroupRepository.GetAll(productGroupId, menuId, collectionId, searchString, pageNumber, pageSize);
            return Ok(data);
        }

        [HttpGet("GroupItem")]
        public IActionResult GetProductGroupWithGroupItem(int? productGroupId, int? menuId, int? collectionId, string? searchString, int pageNumber = 1, int pageSize = 10)
        {
            var data = _unitOfWork.ProductGroupRepository.GetProductGroupWithGroupItem(productGroupId, menuId, collectionId, searchString, pageNumber, pageSize);
            return Ok(data);
        }

        [HttpPost]
        public IActionResult Add(ProductGroupCreateDTO productCreateDTO)
        {
            
            var data = _mapper.Map<ProductGroup>(productCreateDTO);
            _unitOfWork.ProductGroupRepository.Add(data);
            _unitOfWork.Save();
            return CreatedAtAction(nameof(Get), new { data });
        }

        [HttpPut("{productGroupId}")]
        public IActionResult Update(int productGroupId, ProductGroupUpdateDTO productGroupUpdateDTO)
        {
            var data = _unitOfWork.ProductGroupRepository.Find(c => c.ProductGroupID == productGroupId).FirstOrDefault();
            if (data == null) return NotFound();

            _mapper.Map(productGroupUpdateDTO, data);

            _unitOfWork.ProductGroupRepository.Update(data);
            _unitOfWork.Save();
            return Ok(data);
        }

        [HttpDelete("{productGroupId}")]
        public IActionResult Delete(int productGroupId)
        {
            var data = _unitOfWork.ProductGroupRepository.Find(c => c.ProductGroupID == productGroupId).FirstOrDefault();
            if (data == null) return NotFound();

            data.IsDeleted = true;
            _unitOfWork.ProductGroupRepository.Update(data);
            _unitOfWork.Save();
            return Ok();
        }
    }
}