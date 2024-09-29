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
    public class StoresController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IStoreService _storeService;

        public StoresController(IMapper mapper, IStoreService storeService)
        {
            _mapper = mapper;
            _storeService = storeService;
        }

        [HttpGet]
        [AllowAnonymous]
        public ActionResult Get(int? storeId, int? brandId, string? searchString, int pageNumber = 1, int pageSize = 10)
        {
            try
            {
                var data = _storeService.GetAll(storeId, brandId, searchString, pageNumber, pageSize);
                return Ok(data);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpGet("StoreStaffs/{storeId}")]
        public ActionResult GetStoreStaffs(int storeId, Guid userId, string? searchString, int pageNumber = 1, int pageSize = 10)
        {
            try
            {
                var data = _storeService.GetStoreWithStaffs(storeId, userId, searchString, pageNumber, pageSize);
                return Ok(data);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpGet("StoreMenus")]
        public ActionResult GetStoreMenus(int? storeId, int? brandId, string? searchString, int pageNumber = 1, int pageSize = 10)
        {
            try
            {
                var data = _storeService.GetStoreWithMenus(storeId, brandId, searchString, pageNumber, pageSize);
                return Ok(data);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpGet("StoreCollections")]
        public IActionResult GetStoreCollection(int? storeId, int? brandId, string? searchString, int pageNumber = 1, int pageSize = 10)
        {
            try
            {
                var data = _storeService.GetStoreWithCollections(storeId, brandId, searchString, pageNumber, pageSize);
                return Ok(data);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpPost]
        [Authorize(Roles = SD.Role_BrandManager)]
        public IActionResult Add(StoreCreateDTO storeCreateDTO)
        {
            try
            {
                var data = _storeService.Add(storeCreateDTO);
                return CreatedAtAction(nameof(Get), data);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpPut("{storeId}")]
        public IActionResult Update(int storeId, StoreUpdateDTO storeUpdateDTO)
        {
            try
            {
                var data = _storeService.Update(storeId, storeUpdateDTO);
                return Ok(data);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpDelete("{storeId}")]
        [Authorize(Roles = SD.Role_BrandManager)]
        public IActionResult Delete(int storeId)
        {
            try
            {
                _storeService.Delete(storeId);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }
    }
}