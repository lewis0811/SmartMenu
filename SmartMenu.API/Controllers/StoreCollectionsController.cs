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
    public class StoreCollectionsController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IStoreCollectionService _storeCollectionService;

        public StoreCollectionsController(IUnitOfWork unitOfWork, IMapper mapper, IStoreCollectionService storeCollectionService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _storeCollectionService = storeCollectionService;
        }

        [HttpGet]
        public ActionResult Get(int? storeCollectionId, int? collectionId, int? storeId, string? searchString, int pageNumber = 1, int pageSize = 10)
        {
            try
            {
                var data = _storeCollectionService.GetAll(storeCollectionId, collectionId, storeId, searchString, pageNumber, pageSize);
                return Ok(data);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpPost]
        public ActionResult Add(StoreCollectionCreateDTO storeCollectionCreateDTO)
        {
            try
            {
                var data = _storeCollectionService.Add(storeCollectionCreateDTO);
                return CreatedAtAction(nameof(Get), data);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{storeCollectionID}")]
        public ActionResult Update(int storeCollectionId, StoreCollectionCreateDTO storeCollectionCreateDTO)
        {
            try
            {
                var data = _storeCollectionService.Update(storeCollectionId, storeCollectionCreateDTO);
                return Ok(data);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("{storeCollectionID}")]
        public ActionResult Delete(int storeCollectionId)
        {
            try
            {
                _storeCollectionService.Delete(storeCollectionId);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
