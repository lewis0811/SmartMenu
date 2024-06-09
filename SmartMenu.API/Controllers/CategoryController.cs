﻿using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using SmartMenu.Domain.Models;
using SmartMenu.Domain.Models.DTO;
using SmartMenu.Domain.Repository;

namespace SmartMenu.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public CategoryController(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        [HttpGet]
        public IActionResult Get(int? categoryId, string? searchString, int pageNumber = 1, int pageSize = 10)
        {
            var data = _unitOfWork.CategoryRepository.GetAll(categoryId, searchString, pageNumber, pageSize);
            return Ok(data);
        }
        [HttpPost]
        public IActionResult Add(CategoryCreateDTO categoryCreateDTO)
        {
            var data = _mapper.Map<Category>(categoryCreateDTO);
            _unitOfWork.CategoryRepository.Add(data);
            _unitOfWork.Save();
            return CreatedAtAction(nameof(Get), new { data });
        }

        [HttpPut]
        public IActionResult Update(int categoryId, CategoryCreateDTO categoryCreateDTO)
        {
            var data = _unitOfWork.CategoryRepository.Find(c => c.CategoryID == categoryId).FirstOrDefault();
            if (data == null) return NotFound();
            _mapper.Map(categoryCreateDTO, data);
            _unitOfWork.CategoryRepository.Update(data);
            _unitOfWork.Save();
            return Ok(data);
        }

        [HttpDelete]
        public IActionResult Delete(int categoryId)
        {
            var data = _unitOfWork.CategoryRepository.Find(c => c.CategoryID == categoryId).FirstOrDefault();
            if (data == null) return NotFound();

            data.IsDeleted = true;
            _unitOfWork.CategoryRepository.Update(data);
            _unitOfWork.Save();
            return Ok();
        }
    }
}
