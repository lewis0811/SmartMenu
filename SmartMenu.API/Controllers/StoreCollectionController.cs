using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using SmartMenu.Domain.Models;
using SmartMenu.Domain.Models.DTO;
using SmartMenu.Domain.Repository;

namespace SmartMenu.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StoreCollectionController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public StoreCollectionController(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        [HttpGet]
        public IActionResult Get(int? storeCollectionId, int? collectionId, int? storeId, string? searchString, int pageNumber = 1, int pageSize = 10)
        {
            var data = _unitOfWork.StoreCollectionRepository.GetAll(storeCollectionId, collectionId, storeId, searchString, pageNumber, pageSize);
            return Ok(data);
        }
        [HttpPost]
        public IActionResult Add(StoreCollectionCreateDTO storeCollectionCreateDTO)
        {
            var data = _mapper.Map<StoreCollection>(storeCollectionCreateDTO);
            _unitOfWork.StoreCollectionRepository.Add(data);
            _unitOfWork.Save();
            return CreatedAtAction(nameof(Get), new { data });
        }

        [HttpPut]
        public IActionResult Update(int storeCollectionID, StoreCollectionCreateDTO storeCollectionCreateDTO)
        {
            var data = _unitOfWork.StoreCollectionRepository.Find(c => c.StoreCollectionID == storeCollectionID).FirstOrDefault();
            if (data == null) return NotFound();
            _mapper.Map(storeCollectionCreateDTO, data);
            _unitOfWork.StoreCollectionRepository.Update(data);
            _unitOfWork.Save();
            return Ok(data);
        }

        [HttpDelete]
        public IActionResult Delete(int storeCollectionID)
        {
            var data = _unitOfWork.StoreCollectionRepository.Find(c => c.StoreCollectionID == storeCollectionID).FirstOrDefault();
            if (data == null) return NotFound();

            data.IsDeleted = true;
            _unitOfWork.StoreCollectionRepository.Update(data);
            _unitOfWork.Save();
            return Ok();
        }
    }
}
