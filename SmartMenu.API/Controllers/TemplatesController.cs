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
    public class TemplatesController : ControllerBase
    {
        private readonly ITemplateService _templateService;

        public TemplatesController( ITemplateService templateService)
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
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("Layers")]
        public ActionResult GetLayers(int? templateId, int? brandId, string? searchString, int pageNumber = 1, int pageSize = 10)
        {
            try
            {
                var data = _templateService.GetAllWithLayers(templateId,  brandId, searchString, pageNumber, pageSize);
                return Ok(data);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        public IActionResult Add(TemplateCreateDTO templateCreateDTO)
        {
            try
            {
                var data = _templateService.Add(templateCreateDTO);

                return CreatedAtAction(nameof(Get), data);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{templateId}")]
        public IActionResult Update(int templateId, TemplateUpdateDTO templateUpdateDTO)
        {
            try
            {
                var data = _templateService.Update(templateId, templateUpdateDTO);
                return Ok(data);
            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{templateId}/image")]
        public IActionResult Update(int templateId, string TemplateImgPath)
        {
            try
            {
                var data = _templateService.Update(templateId, TemplateImgPath);
                return Ok(data);
            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("{templateId}")]
        public IActionResult Delete(int templateId)
        {
            _templateService.Delete(templateId);

            return Ok();
        }
    }
}