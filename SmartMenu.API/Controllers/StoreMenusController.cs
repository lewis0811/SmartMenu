using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using SmartMenu.Domain.Models;
using SmartMenu.Domain.Models.DTO;
using SmartMenu.Domain.Repository;
using SmartMenu.Service.Interfaces;
using SmartMenu.Service.Services;

namespace SmartMenu.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StoreMenusController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IStoreMenuService _storeMenuService;

        public StoreMenusController(IUnitOfWork unitOfWork, IMapper mapper, IStoreMenuService storeMenuService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _storeMenuService = storeMenuService;
        }

        [HttpGet]
        public ActionResult Get(int? storeMenuId, int? storeId, int? menuId, string? searchString, int pageNumber = 1, int pageSize = 10)
        {
            try
            {
                var data = _storeMenuService.GetAll(storeMenuId, storeId, menuId, searchString, pageNumber, pageSize);
                return Ok(data);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }
        [HttpPost]
        public ActionResult Add(StoreMenuCreateDTO storeMenuCreateDTO)
        {

            try
            {
                var data = _storeMenuService.Add(storeMenuCreateDTO);
                return CreatedAtAction(nameof(Get), data);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpPut("{storeMenuId}")]
        public ActionResult Update(int storeMenuId, StoreMenuCreateDTO storeMenuCreateDTO)
        {
            try
            {
                var data = _storeMenuService.Update(storeMenuId, storeMenuCreateDTO);
                return Ok(data);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpDelete("{storeMenuId}")]
        public ActionResult Delete(int storeMenuId)
        {
            try
            {
                _storeMenuService.Delete(storeMenuId);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpDelete("{storeMenuId}/v2")]
        public ActionResult DeleteV2(int storeMenuId)
        {
            try
            {
                _storeMenuService.DeleteV2(storeMenuId);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }
    }
}
