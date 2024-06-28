using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using SmartMenu.Domain.Models;
using SmartMenu.Domain.Models.DTO;
using SmartMenu.Domain.Repository;

namespace SmartMenu.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StoreCollectionsController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public StoreCollectionsController(IUnitOfWork unitOfWork, IMapper mapper)
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

        [HttpPut("{storeCollectionID}")]
        public IActionResult Update(int storeCollectionId, StoreCollectionCreateDTO storeCollectionCreateDTO)
        {
            var data = _unitOfWork.StoreCollectionRepository.Find(c => c.StoreCollectionId == storeCollectionId).FirstOrDefault();
            if (data == null) return NotFound();
            _mapper.Map(storeCollectionCreateDTO, data);
            _unitOfWork.StoreCollectionRepository.Update(data);
            _unitOfWork.Save();
            return Ok(data);
        }

        [HttpDelete("{storeCollectionID}")]
        public IActionResult Delete(int storeCollectionId)
        {
            var data = _unitOfWork.StoreCollectionRepository.Find(c => c.StoreCollectionId == storeCollectionId).FirstOrDefault();
            if (data == null) return NotFound();

            data.IsDeleted = true;
            _unitOfWork.StoreCollectionRepository.Update(data);
            _unitOfWork.Save();
            return Ok();
        }
    }
}
