using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using SmartMenu.Domain.Models;
using SmartMenu.Domain.Models.DTO;
using SmartMenu.Domain.Repository;

namespace SmartMenu.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BrandsController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public BrandsController(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        [HttpGet]
        public IActionResult Get(int? brandId, string? searchString, int pageNumber = 1, int pageSize = 10)
        {
            var data = _unitOfWork.BrandRepository.GetAll(brandId, searchString, pageNumber, pageSize);
            return Ok(data);
        }
        [HttpGet("BrandStaff")]
        public IActionResult GetBranchWithBrandStaff(int? brandId, string? searchString, int pageNumber = 1, int pageSize = 10)
        {
            var data = _unitOfWork.BrandRepository.GetBranchWithBrandStaff(brandId, searchString, pageNumber, pageSize);
            return Ok(data);
        }
        [HttpGet("BrandStore")]
        public IActionResult GetBranchWithStore(int? brandId, string? searchString, int pageNumber = 1, int pageSize = 10)
        {
            var data = _unitOfWork.BrandRepository.GetBranchWithStore(brandId, searchString, pageNumber, pageSize);
            return Ok(data);
        }
        [HttpGet("BrandProduct")]
        public IActionResult GetBranchWithProduct(int? brandId, string? searchString, int pageNumber = 1, int pageSize = 10)
        {
            try
            {
                var data = _unitOfWork.BrandRepository.GetBranchWithProduct(brandId, searchString, pageNumber, pageSize);
                return Ok(data);
            }
            catch (Exception ex)
            {

                return BadRequest(ex);
            }

        }
        [HttpPost]
        public IActionResult Add(BrandCreateDTO brandCreateDTO)
        {
            var data = _mapper.Map<Brand>(brandCreateDTO);
            _unitOfWork.BrandRepository.Add(data);
            _unitOfWork.Save();
            return CreatedAtAction(nameof(Get), new { data });
        }

        [HttpPut("{brandId}")]
        public IActionResult Update(int brandId, BrandCreateDTO brandCreateDTO) 
        {
            var data = _unitOfWork.BrandRepository.Find(c => c.BrandId == brandId && c.IsDeleted == false && c.IsDeleted == false).FirstOrDefault();
            if (data == null) return BadRequest("Brand not found or deleted");

            _mapper.Map(brandCreateDTO, data);

            _unitOfWork.BrandRepository.Update(data);
            _unitOfWork.Save();
            return Ok(data);
        }

        [HttpDelete("{brandId}")]
        public IActionResult Delete(int brandId)
        {
            var data = _unitOfWork.BrandRepository.Find(c => c.BrandId == brandId && c.IsDeleted == false).FirstOrDefault();
            if (data == null) return BadRequest("Brand not found or deleted");

            data.IsDeleted = true;
            _unitOfWork.BrandRepository.Update(data);
            _unitOfWork.Save();
            return Ok();
        }
    }
}
