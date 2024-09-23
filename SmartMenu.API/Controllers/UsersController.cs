using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SmartMenu.API.Ultility;
using SmartMenu.Domain.Models;
using SmartMenu.Domain.Models.DTO;
using SmartMenu.Domain.Repository;
using SmartMenu.Service.Interfaces;

namespace SmartMenu.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = SD.Role_Admin + "," + SD.Role_BrandManager + "," + SD.Role_StoreManager)]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet]
        public IActionResult Get(Guid? userId, string? searchString, int pageNumber = 1, int pageSize = 10, bool isDeleted = false)
        {
            try
            {
                var data = _userService.GetAll(userId, isDeleted, searchString, pageNumber, pageSize);
                data ??= Enumerable.Empty<User>();

                return Ok(data);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });

            }
        }

        [HttpPut("{userId}")]
        public IActionResult Update(Guid userId, UserUpdateDTO userUpdateDTO)
        {
            try
            {
                var data = _userService.Update(userId, userUpdateDTO);

                return Ok(data);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpDelete("{userId}")]
        [Authorize(Roles = SD.Role_Admin)]
        public IActionResult Delete(Guid userId)
        {
            try
            {
                _userService.Delete(userId);
                return Ok();
            }
            catch (Exception ex)
            {

                return BadRequest(new { error = ex.Message });
            }
        }

    }
}
