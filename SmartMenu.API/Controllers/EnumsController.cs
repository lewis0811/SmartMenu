using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SmartMenu.Domain.Models.Enum;
using SmartMenu.Service.Interfaces;

namespace SmartMenu.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EnumsController : ControllerBase
    {
        private readonly IEnumService _enumService;

        public EnumsController(IEnumService enumService)
        {
            _enumService = enumService;
        }

        [HttpGet("BoxItemType")]
        public IActionResult GetBoxItemType()
        {
            var enums = _enumService.GetBoxItemType();
            return Ok(enums);
        }

        [HttpGet("BoxType")]
        public IActionResult GetBoxType() 
        {
            var enums = _enumService.GetBoxType();
            return Ok(enums);
        }

        [HttpGet("LayerType")]
        public IActionResult GetLayerType()
        {
            var enums = _enumService.GetLayerType();
            return Ok(enums);
        }

        [HttpGet("ProductSizeType")]
        public IActionResult GetProductSizeType()
        {
            var enums = _enumService.GetProductSizeType();
            return Ok(enums);
        }

        [HttpGet("RoleType")]
        public IActionResult GetRoleType()
        {
            var enums = _enumService.GetRoleType();
            return Ok(enums);
        }

        [HttpGet("TemplateType")]
        public IActionResult GetTemplateType()
        {
            var enums = _enumService.GetTemplateType();
            return Ok(enums);
        }
    }
}
