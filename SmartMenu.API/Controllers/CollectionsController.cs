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
    public class CollectionsController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ICollectionService _collectionService;

        public CollectionsController(IUnitOfWork unitOfWork, IMapper mapper, ICollectionService collectionService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _collectionService = collectionService;
        }
        [HttpGet]
        public ActionResult Get(int? collectionId, int? brandId, string? searchString, int pageNumber = 1, int pageSize = 10)
        {
            try
            {
                var data = _collectionService.GetAll(collectionId, brandId, searchString, pageNumber, pageSize);
                return Ok(data);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("ProductGroup")]
        public ActionResult GetCollectionProductGroup(int? collectionId, int? brandId, string? searchString, int pageNumber = 1, int pageSize = 10)
        {
            try
            {
                var data = _collectionService.GetCollectionWithProductGroup(collectionId, brandId, searchString, pageNumber, pageSize);
                return Ok(data);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpPost]
        public ActionResult Add(CollectionCreateDTO collectionCreateDTO)
        {
            try
            {
                var data = _collectionService.Add(collectionCreateDTO);
                return CreatedAtAction(nameof(Get), data);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{collectionId}")]
        public ActionResult Update(int collectionId, CollectionUpdateDTO collectionUpdateDTO)
        {
            try
            {
                var data = _collectionService.Update(collectionId, collectionUpdateDTO);
                return Ok(data);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("{collectionId}")]
        public ActionResult Delete(int collectionId)
        {
            try
            {
                _collectionService.Delete(collectionId);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
