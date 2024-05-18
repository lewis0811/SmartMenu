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
    public class BrandStaffController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public BrandStaffController(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        [HttpGet]
        public IActionResult Get(int? brandStaffId, int? brandId, Guid? userId, string? searchString, int pageNumber = 1, int pageSize = 10)
        {
            var data = _unitOfWork.BrandStaffRepository.GetAll(brandStaffId, brandId, userId, searchString, pageNumber, pageSize);
            return Ok(data);
        }
        [HttpPost]
        public IActionResult Add(BrandStaffCreateDTO brandStaffCreateDTO)
        {
            var data = _mapper.Map<BrandStaff>(brandStaffCreateDTO);
            _unitOfWork.BrandStaffRepository.Add(data);
            _unitOfWork.Save();
            return CreatedAtAction(nameof(Get), new { data });
        }

        [HttpPut]
        public IActionResult Update(int brandStaffId, BrandStaffCreateDTO brandStaffCreateDTO)
        {
            var data = _unitOfWork.BrandStaffRepository.Find(c => c.BrandStaffID == brandStaffId).FirstOrDefault();
            if (data == null) return NotFound();
            _mapper.Map(brandStaffCreateDTO, data);
            _unitOfWork.BrandStaffRepository.Update(data);
            _unitOfWork.Save();
            return Ok(data);
        }

        [HttpDelete]
        public IActionResult Delete(int brandStaffId)
        {
            var data = _unitOfWork.BrandStaffRepository.Find(c => c.BrandStaffID == brandStaffId).FirstOrDefault();
            if (data == null) return NotFound();

            data.IsDeleted = true;
            _unitOfWork.BrandStaffRepository.Update(data);
            _unitOfWork.Save();
            return Ok();
        }
    }
}
