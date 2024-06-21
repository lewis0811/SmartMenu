using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using SmartMenu.Domain.Models;
using SmartMenu.Domain.Models.DTO;
using SmartMenu.Domain.Repository;

namespace SmartMenu.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CollectionsController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public CollectionsController(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        [HttpGet]
        public IActionResult Get(int? collectionId, int? brandId, string? searchString, int pageNumber = 1, int pageSize = 10)
        {
            var data = _unitOfWork.CollectionRepository.GetAll(collectionId, brandId, searchString, pageNumber, pageSize);
            data ??= Enumerable.Empty<Collection>();

            return Ok(data);
        }

        [HttpGet("ProductGroup")]
        public IActionResult GetCollectionProductGroup(int? collectionId, int? brandId, string? searchString, int pageNumber = 1, int pageSize = 10)
        {
            var data = _unitOfWork.CollectionRepository.GetCollectionWithProductGroup(collectionId, brandId, searchString, pageNumber, pageSize);
            return Ok(data);
        }
        [HttpPost]
        public IActionResult Add(CollectionCreateDTO collectionCreateDTO)
        {
            var data = _mapper.Map<Collection>(collectionCreateDTO);
            _unitOfWork.CollectionRepository.Add(data);
            _unitOfWork.Save();
            return CreatedAtAction(nameof(Get), new { data });
        }

        [HttpPut("{collectionId}")]
        public IActionResult Update(int collectionId, CollectionUpdateDTO collectionUpdateDTO)
        {
            var data = _unitOfWork.CollectionRepository.Find(c => c.CollectionID == collectionId).FirstOrDefault();
            if (data == null) return NotFound();

            _mapper.Map(collectionUpdateDTO, data);

            _unitOfWork.CollectionRepository.Update(data);
            _unitOfWork.Save();
            return Ok(data);
        }

        [HttpDelete("{collectionId}")]
        public IActionResult Delete(int collectionId)
        {
            var data = _unitOfWork.CollectionRepository.Find(c => c.CollectionID == collectionId).FirstOrDefault();
            if (data == null) return NotFound();

            data.IsDeleted = true;
            _unitOfWork.CollectionRepository.Update(data);
            _unitOfWork.Save();
            return Ok();
        }
    }
}
