using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartMenu.API.Ultility;
using SmartMenu.Domain.Models;
using SmartMenu.Domain.Repository;

namespace SmartMenu.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = SD.Role_Admin)]
    
    public class RolesController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;

        public RolesController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        public IActionResult Get()
        {
            var roles = _unitOfWork.RoleRepository.GetAll();
            if (roles == null || roles.ToList().Count < 3)
            {
                var newRoles = new List<Role>()
                {
                    new Role() { RoleName = SD.Role_Admin },
                    new Role() { RoleName = SD.Role_StoreManager },
                    new Role() { RoleName = SD.Role_BrandManager }
                };
                _unitOfWork.RoleRepository.AddRange(newRoles);
                _unitOfWork.Save();
                return Ok(newRoles);
            }
            return Ok(roles);
        }
    }
}