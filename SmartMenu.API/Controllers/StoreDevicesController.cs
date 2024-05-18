using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using SmartMenu.Domain.Models;
using SmartMenu.Domain.Models.DTO;
using SmartMenu.Domain.Repository;

namespace SmartMenu.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StoreDevicesController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;

        public StoreDevicesController(IMapper mapper, IUnitOfWork unitOfWork)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        public IActionResult Get(int? storeDeviceId, int? storeId, int? displayId, string? searchString, int pageNumber = 1, int pageSize = 10)
        {
            var data = _unitOfWork.StoreDeviceRepository.GetAll(storeDeviceId, storeId, displayId, searchString, pageNumber, pageSize).ToList();
            if (data.Count == 0) return NotFound();

            return Ok(data);
        }

        [HttpPost]
        public IActionResult Add(StoreDeviceCreateDTO storeDeviceCreateDTO)
        {
            var data = _mapper.Map<StoreDevice>(storeDeviceCreateDTO);
            _unitOfWork.StoreDeviceRepository.Add(data);
            _unitOfWork.Save();
            return CreatedAtAction(nameof(Get), data);
        }

        [HttpPut("{storeDeviceId}")]
        public IActionResult Update(int storeDeviceId, StoreDeviceUpdateDTO storeDeviceUpdateDTO)
        {
            var data = _unitOfWork.StoreDeviceRepository.Find(c => c.StoreDeviceID == storeDeviceId).FirstOrDefault();
            if (data == null || data.IsDeleted == true) return NotFound();

            _mapper.Map(storeDeviceUpdateDTO, data);
            _unitOfWork.StoreDeviceRepository.Update(data);
            _unitOfWork.Save();
            return Ok(data);
        }

        [HttpDelete("{storeDeviceId}")]
        public IActionResult Delete(int storeDeviceId)
        {
            var data = _unitOfWork.StoreDeviceRepository.Find(c => c.StoreDeviceID == storeDeviceId).FirstOrDefault();
            if (data == null || data.IsDeleted == true) return NotFound();

            data.IsDeleted = true;
            _unitOfWork.StoreDeviceRepository.Update(data);
            _unitOfWork.Save();
            return Ok();
        }
    }
}