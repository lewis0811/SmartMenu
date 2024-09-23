using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartMenu.API.Ultility;
using SmartMenu.Domain.Models;
using SmartMenu.Domain.Models.DTO;
using SmartMenu.Domain.Repository;
using SmartMenu.Service.Interfaces;
using SmartMenu.Service.Services;

namespace SmartMenu.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = SD.Role_BrandManager + "," + SD.Role_StoreManager)]
    public class CollectionsController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly ICollectionService _collectionService;

        public CollectionsController(IMapper mapper, ICollectionService collectionService)
        {
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
                return BadRequest(new { error = ex.Message });
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
                return BadRequest(new { error = ex.Message });
            }
        }
        [HttpPost]
        [Authorize(Roles = SD.Role_BrandManager)]
        public ActionResult Add(CollectionCreateDTO collectionCreateDTO)
        {
            try
            {
                var data = _collectionService.Add(collectionCreateDTO);
                return CreatedAtAction(nameof(Get), data);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpPut("{collectionId}")]
        [Authorize(Roles = SD.Role_BrandManager)]
        public ActionResult Update(int collectionId, CollectionUpdateDTO collectionUpdateDTO)
        {
            try
            {
                var data = _collectionService.Update(collectionId, collectionUpdateDTO);
                return Ok(data);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpDelete("{collectionId}")]
        [Authorize(Roles = SD.Role_BrandManager)]
        public ActionResult Delete(int collectionId)
        {
            try
            {
                _collectionService.Delete(collectionId);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }
    }
}
