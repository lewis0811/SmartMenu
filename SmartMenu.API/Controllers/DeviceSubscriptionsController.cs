using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SmartMenu.Domain.Models.DTO;
using SmartMenu.Service.Interfaces;

namespace SmartMenu.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DeviceSubscriptionsController : ControllerBase
    {
        private readonly IDeviceSubscriptionService _deviceSubscriptionService;

        public DeviceSubscriptionsController(IDeviceSubscriptionService deviceSubscriptionService)
        {
            _deviceSubscriptionService = deviceSubscriptionService;
        }

        [HttpGet]
        public IActionResult Get(int? deviceSubscriptionId, int? storeDeviceId, string? searchString, int pageNumber = 1, int pageSize = 10) {
            try
            {
                var data = _deviceSubscriptionService.GetAll(deviceSubscriptionId, storeDeviceId, searchString, pageNumber, pageSize);
                return Ok(data);
            }
            catch (Exception ex)
            {

                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> AddAsync(DeviceSubscriptionCreateDTO deviceSubscriptionCreateDTO)
        {
            try
            {
                var data = await _deviceSubscriptionService.AddDeviceSubscription(deviceSubscriptionCreateDTO);
                return CreatedAtAction(nameof(Get), data);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpPut("{deviceSubscriptionId}")]
        public async Task<IActionResult> UpdateAsync(int deviceSubscriptionId, DeviceSubscriptionUpdateDTO deviceSubscriptionUpdateDTO)
        {
            try
            {
                var data =  await _deviceSubscriptionService.UpdateDeviceSubscription(deviceSubscriptionId ,deviceSubscriptionUpdateDTO);
                return Ok(data);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpDelete("{deviceSubscriptionId}")]
        public IActionResult Delete(int deviceSubscriptionId)
        {
            try
            {
                _deviceSubscriptionService.DeleteDeviceSubscription(deviceSubscriptionId);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }
    }
}
