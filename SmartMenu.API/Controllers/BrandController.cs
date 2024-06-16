using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using SmartMenu.Domain.Models;
using SmartMenu.Domain.Models.DTO;
using SmartMenu.Domain.Repository;

namespace SmartMenu.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BrandController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public BrandController(IUnitOfWork unitOfWork, IMapper mapper)
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
        [HttpGet("brandStore")]
        public IActionResult GetBranchWithStore(int? brandId, string? searchString, int pageNumber = 1, int pageSize = 10)
        {
            var data = _unitOfWork.BrandRepository.GetBranchWithStore(brandId, searchString, pageNumber, pageSize);
            return Ok(data);
        }
        [HttpGet("BrandProduct")]
        public IActionResult GetBranchWithProduct(int? brandId, string? searchString, int pageNumber = 1, int pageSize = 10)
        {
            var data = _unitOfWork.BrandRepository.GetBranchWithProduct(brandId, searchString, pageNumber, pageSize);
            return Ok(data);
        }
        [HttpPost]
        public IActionResult Add(BrandCreateDTO brandCreateDTO)
        {
            var data = _mapper.Map<Brand>(brandCreateDTO);
            _unitOfWork.BrandRepository.Add(data);
            _unitOfWork.Save();
            return CreatedAtAction(nameof(Get), new { data });
        }

        [HttpPut]
        public IActionResult Update(int brandId, BrandCreateDTO brandCreateDTO)
        {
            var data = _unitOfWork.BrandRepository.Find(c => c.BrandID == brandId).FirstOrDefault();
            if (data == null) return NotFound();

            _mapper.Map(brandCreateDTO, data);

            _unitOfWork.BrandRepository.Update(data);
            _unitOfWork.Save();
            return Ok(data);
        }

        [HttpDelete]
        public IActionResult Delete(int brandId)
        {
            var data = _unitOfWork.BrandRepository.Find(c => c.BrandID == brandId).FirstOrDefault();
            if (data == null) return NotFound();

            data.IsDeleted = true;
            _unitOfWork.BrandRepository.Update(data);
            _unitOfWork.Save();
            return Ok();
        }
    }
}
