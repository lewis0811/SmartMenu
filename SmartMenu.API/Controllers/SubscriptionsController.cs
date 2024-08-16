using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SmartMenu.Domain.Models.DTO;
using SmartMenu.Service.Interfaces;

namespace SmartMenu.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SubscriptionsController : ControllerBase
    {
        private readonly ISubscriptionService _subscriptionService;

        public SubscriptionsController(ISubscriptionService subscriptionService)
        {
            _subscriptionService = subscriptionService;
        }

        [HttpGet]
        public IActionResult Get(int? subscriptionId, string? searchString, int pageNumber = 1, int pageSize = 10)
        {
            try
            {
                var data = _subscriptionService.GetAll(subscriptionId, searchString, pageNumber, pageSize);
                return Ok(data);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Post(SubscriptionCreateDTO subscriptionCreateDTO)
        {
            try
            {
                var data = await _subscriptionService.AddSubscription(subscriptionCreateDTO);
                return CreatedAtAction(nameof(Get), data);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpPut("{subscriptionId}")]
        public async Task<IActionResult> UpdateAsync(int subscriptionId, SubscriptionUpdateDTO subscriptionUpdateDTO)
        {
            try
            {
                var data = await _subscriptionService.UpdateSubscription(subscriptionId, subscriptionUpdateDTO);
                return Ok(data);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpDelete("{subscriptionId}")]
        public IActionResult Delete(int subscriptionId)
        {
            try
            {
                _subscriptionService.DeleteSubscription(subscriptionId);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }
    }
}