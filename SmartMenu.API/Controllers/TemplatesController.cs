using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Azure;
using SmartMenu.API.Ultility;
using SmartMenu.Domain.Models;
using SmartMenu.Domain.Models.DTO;
using SmartMenu.Domain.Repository;
using SmartMenu.Service.Interfaces;

namespace SmartMenu.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = SD.Role_BrandManager + "," + SD.Role_StoreManager)]
    public class TemplatesController : ControllerBase
    {
        private readonly ITemplateService _templateService;

        public TemplatesController(ITemplateService templateService)
        {
            _templateService = templateService;
        }

        [HttpGet]
        public ActionResult Get(int? templateId, int? brandId, string? searchString, int pageNumber = 1, int pageSize = 10)
        {
            try
            {
                var data = _templateService.GetAll(templateId, brandId, searchString, pageNumber, pageSize);
                return Ok(data);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpGet("Layers")]
        public ActionResult GetLayers(int? templateId, int? brandId, string? searchString, int pageNumber = 1, int pageSize = 10)
        {
            try
            {
                var data = _templateService.GetAllWithLayers(templateId, brandId, searchString, pageNumber, pageSize);
                return Ok(data);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpPost]
        [Authorize(Roles = SD.Role_BrandManager)]
        public IActionResult Add(TemplateCreateDTO templateCreateDTO)
        {
            try
            {
                var data = _templateService.Add(templateCreateDTO);

                return CreatedAtAction(nameof(Get), data);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpPut("{templateId}")]
        [Authorize(Roles = SD.Role_BrandManager)]
        public IActionResult Update(int templateId, TemplateUpdateDTO templateUpdateDTO)
        {
            try
            {
                var data = _templateService.Update(templateId, templateUpdateDTO);
                return Ok(data);
            }
            catch (Exception ex)
            {

                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpPut("{templateId}/image")]
        [Authorize(Roles = SD.Role_BrandManager)]
        public IActionResult Update(int templateId, string TemplateImgPath)
        {
            try
            {
                var data = _templateService.Update(templateId, TemplateImgPath);
                return Ok(data);
            }
            catch (Exception ex)
            {

                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpDelete("{templateId}")]
        [Authorize(Roles = SD.Role_BrandManager)]
        public IActionResult Delete(int templateId)
        {
            _templateService.Delete(templateId);

            return Ok();
        }
    }
}