using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartMenu.API.Ultility;
using SmartMenu.Domain.Models;
using SmartMenu.Domain.Models.DTO;
using SmartMenu.Domain.Repository;
using SmartMenu.Service.Interfaces;
using SmartMenu.Service.Services;

namespace SmartMenu.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = SD.Role_Admin + "," + SD.Role_BrandManager + "," + SD.Role_StoreManager)]
    public class BrandStaffsController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IBrandStaffService _brandStaffService;

        public BrandStaffsController(IMapper mapper, IBrandStaffService brandStaffService)
        {
            _mapper = mapper;
            _brandStaffService = brandStaffService;
        }

        [HttpGet]
        public ActionResult Get(int? brandStaffId, int? brandId, Guid? userId, string? searchString, int pageNumber = 1, int pageSize = 10)
        {
            try
            {
                var data = _brandStaffService.GetAll(brandStaffId, brandId, userId, searchString, pageNumber, pageSize);
                return Ok(data);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            };
        }
        [HttpPost]
        [Authorize(Roles = SD.Role_Admin + "," + SD.Role_BrandManager)]
        public ActionResult Add(BrandStaffCreateDTO brandStaffCreateDTO)
        {
            try
            {
                var data = _brandStaffService.Add(brandStaffCreateDTO);
                return CreatedAtAction(nameof(Get), data);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpPut("{brandStaffId}")]
        public ActionResult Update(int brandStaffId, BrandStaffUpdateDTO brandStaffUpdateDTO)
        {
            try
            {
                var data = _brandStaffService.Update(brandStaffId, brandStaffUpdateDTO);
                return Ok(data);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpDelete("{brandStaffId}")]
        [Authorize(Roles = SD.Role_Admin + "," + SD.Role_BrandManager)]
        public ActionResult Delete(int brandStaffId)
        {
            try
            {
                _brandStaffService.Delete(brandStaffId);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }
    }
}
