﻿using AutoMapper;
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
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ITemplateService _templateService;

        public TemplatesController(IMapper mapper, IUnitOfWork unitOfWork, ITemplateService templateService)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _templateService = templateService;
        }

        [HttpGet]
        public ActionResult Get(int? templateId, string? searchString, int pageNumber = 1, int pageSize = 10)
        {
            try
            {
                var data = _templateService.GetAll(templateId, searchString, pageNumber, pageSize);
                return Ok(data);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("Layers")]
        public ActionResult GetLayers(int? templateId, string? searchString, int pageNumber = 1, int pageSize = 10)
        {
            try
            {
                var data = _templateService.GetAllWithLayers(templateId, searchString, pageNumber, pageSize);
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
            var data = _templateService.Add(templateCreateDTO);

            return CreatedAtAction(nameof(Get), data);
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

        [HttpDelete("{templateId}")]
        public IActionResult Delete(int templateId)
        {
            _templateService.Delete(templateId);

            return Ok();
        }
    }
}