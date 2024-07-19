using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Azure;
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
        private readonly IUnitOfWork _unitOfWork;
        private readonly IStoreDeviceService _storeDeviceService;

        public StoreDevicesController(IMapper mapper, IUnitOfWork unitOfWork, IStoreDeviceService storeDeviceService)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _storeDeviceService = storeDeviceService;
        }

        [HttpGet]
        public IActionResult Get(int? storeDeviceId, int? storeId, string? searchString, int pageNumber = 1, int pageSize = 10)
        {
            var data = _storeDeviceService.GetAll(storeDeviceId, storeId, searchString, pageNumber, pageSize).ToList();
            if (data.Count == 0) return NotFound();

            return Ok(data);
        }

        [HttpGet("displays")]
        public IActionResult GetWithDisplays(int? storeDeviceId, int? storeId, string? searchString, int pageNumber = 1, int pageSize = 10)
        {
            var data = _storeDeviceService.GetAllWithDisplays(storeDeviceId, storeId, searchString, pageNumber, pageSize);
            if (data.ToList().Count == 0) data = Enumerable.Empty<StoreDevice>();

            return Ok(data);
        }

        [HttpPost]
        public IActionResult Add(StoreDeviceCreateDTO storeDeviceCreateDTO)
        {
            var data = _storeDeviceService.Add(storeDeviceCreateDTO);
            return CreatedAtAction(nameof(Get), data);
        }

        [HttpPut("{storeDeviceId}")]
        public IActionResult Update(int storeDeviceId, StoreDeviceUpdateDTO storeDeviceUpdateDTO)
        {
            var data = _storeDeviceService.Update(storeDeviceId, storeDeviceUpdateDTO);
            return Ok(data);
        }

        [HttpPut("{storeDeviceId}/status")]
        public IActionResult UpdateStatus(int storeDeviceId, bool isApproved)
        {
            var data = _storeDeviceService.Update(storeDeviceId, isApproved);
            return Ok(data);
        }

        [HttpDelete("{storeDeviceId}")]
        public IActionResult Delete(int storeDeviceId)
        {
            _storeDeviceService.Delete(storeDeviceId);
            return Ok();
        }
    }
}