using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartMenu.API.Ultility;
using SmartMenu.Domain.Models;
using SmartMenu.Domain.Models.DTO;
using SmartMenu.Domain.Models.Enum;
using SmartMenu.Domain.Repository;
using SmartMenu.Service.Interfaces;
using SmartMenu.Service.Services;

namespace SmartMenu.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = SD.Role_Admin)]
    public class BrandsController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IBrandService _brandService;

        public BrandsController(IUnitOfWork unitOfWork, IMapper mapper, IBrandService brandService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _brandService = brandService;
        }

        [HttpGet]
        public ActionResult Get(int? brandId, string? searchString, int pageNumber = 1, int pageSize = 10)
        {
            try
            {
                var data = _brandService.GetAll(brandId, searchString, pageNumber, pageSize);
                return Ok(data);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("BrandStaff")]
        public ActionResult GetBranchWithBrandStaff(int? brandId, string? searchString, int pageNumber = 1, int pageSize = 10)
        {
            try
            {
                var data = _brandService.GetBranchWithBrandStaff(brandId, searchString, pageNumber, pageSize);
                return Ok(data);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("BrandStore")]
        public ActionResult GetBranchWithStore(int? brandId, string? searchString, int pageNumber = 1, int pageSize = 10)
        {
            try
            {
                var data = _brandService.GetBranchWithStore(brandId, searchString, pageNumber, pageSize);
                return Ok(data);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        //[HttpGet("BrandProduct")]
        //public IActionResult GetBranchWithProduct(int? brandId, string? searchString, int pageNumber = 1, int pageSize = 10)
        //{
        //        var data = _unitOfWork.BrandRepository.GetBranchWithProduct(brandId, searchString, pageNumber, pageSize);
        //        return Ok(data);
        //}

        [HttpPost]
        public ActionResult Add(BrandCreateDTO brandCreateDTO)
        {
            try
            {
                var data = _brandService.Add(brandCreateDTO);
                return CreatedAtAction(nameof(Get), data);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{brandId}")]
        public ActionResult Update(int brandId, BrandUpdateDTO brandUpdateDTO) 
        {
            try
            {
                var data = _brandService.Update(brandId, brandUpdateDTO);
                return Ok(data);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("{brandId}")]
        public ActionResult Delete(int brandId)
        {
            try
            {
                _brandService.Delete(brandId);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
