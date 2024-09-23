using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Azure;
using SmartMenu.API.Ultility;
using SmartMenu.Domain.Models;
using SmartMenu.Domain.Models.DTO;
using SmartMenu.Domain.Repository;
using SmartMenu.Service.Interfaces;

namespace SmartMenu.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StoreDevicesController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IStoreDeviceService _storeDeviceService;

        public StoreDevicesController(IMapper mapper, IStoreDeviceService storeDeviceService)
        {
            _mapper = mapper;
            _storeDeviceService = storeDeviceService;
        }

        [HttpGet]
        public IActionResult Get(int? storeDeviceId, int? storeId, string? searchString, int pageNumber = 1, int pageSize = 10)
        {
            try
            {
                var data = _storeDeviceService.GetAll(storeDeviceId, storeId, searchString, pageNumber, pageSize).ToList();
                if (data.Count == 0) return NotFound();

                return Ok(data);
            }
            catch (Exception ex)
            {

                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpGet("displays")]
        public IActionResult GetWithDisplays(int? storeDeviceId, int? storeId, string? searchString, int pageNumber = 1, int pageSize = 10)
        {
            try
            {
                var data = _storeDeviceService.GetAllWithDisplays(storeDeviceId, storeId, searchString, pageNumber, pageSize);
                if (data.ToList().Count == 0) data = Enumerable.Empty<StoreDevice>();

                return Ok(data);
            }
            catch (Exception ex)
            {

                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpGet("isSubscription")]
        public IActionResult GetIfSubscription(int storeDeviceId)
        {
            try
            {
                var data = _storeDeviceService.IsSubscription(storeDeviceId);

                return Ok(data);
            }
            catch (Exception ex)
            {

                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpPost]
        public IActionResult Add(StoreDeviceCreateDTO storeDeviceCreateDTO)
        {
            try
            {
                var data = _storeDeviceService.Add(storeDeviceCreateDTO);
                return CreatedAtAction(nameof(Get), data);
            }
            catch (Exception ex)
            {

                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpPut("{storeDeviceId}")]
        public IActionResult Update(int storeDeviceId, StoreDeviceUpdateDTO storeDeviceUpdateDTO)
        {
            try
            {
                var data = _storeDeviceService.Update(storeDeviceId, storeDeviceUpdateDTO);
                return Ok(data);
            }
            catch (Exception ex)
            {

                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpPut("{storeDeviceId}/status")]
        public IActionResult UpdateStatus(int storeDeviceId)
        {
            try
            {
                var data = _storeDeviceService.UpdateApprove(storeDeviceId);
                return Ok(data);
            }
            catch (Exception ex)
            {

                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpPut("{storeDeviceId}/ratio-type")]
        public IActionResult UpdateRatioType(int storeDeviceId)
        {
            try
            {
                var data = _storeDeviceService.UpdateRatioType(storeDeviceId);
                return Ok(data);
            }
            catch (Exception ex)
            {

                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpDelete("{storeDeviceId}")]
        public IActionResult Delete(int storeDeviceId)
        {
            try
            {
                _storeDeviceService.Delete(storeDeviceId);
                return Ok();
            }
            catch (Exception ex)
            {

                return BadRequest(new { error = ex.Message });
            }
        }
    }
}