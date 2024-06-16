﻿using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SmartMenu.Domain.Models;
using SmartMenu.Domain.Models.DTO;
using SmartMenu.Domain.Repository;

namespace SmartMenu.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;

        public UserController(IMapper mapper, IUnitOfWork unitOfWork)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        public IActionResult Get(Guid? userId, string? searchString, int pageNumber = 1, int pageSize = 10)
        {
            try
            {
                var data = _unitOfWork.UserRepository
                    .GetAll(userId, searchString, pageNumber, pageSize)
                    .ToList();
                if (data.Count == 0) return NotFound();

                return Ok(data);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut]
        public IActionResult Update(Guid userId, UserUpdateDTO userUpdateDTO)
        {
            try
            {
                if(userUpdateDTO.Password != userUpdateDTO.ConfirmPassword)
                {
                    return BadRequest("Password not match!");
                }
                var data = _unitOfWork.UserRepository.GetByID(userId);
                data.Password = userUpdateDTO.Password;

                _unitOfWork.UserRepository.Update(data);
                _unitOfWork.Save();

                return Ok(data);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
                throw;
            }
        }

    }
}
