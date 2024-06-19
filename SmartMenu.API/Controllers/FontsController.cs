using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using SmartMenu.API.Ultility;
using SmartMenu.Domain.Models;
using SmartMenu.Domain.Models.DTO;
using SmartMenu.Domain.Repository;

namespace SmartMenu.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FontsController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public FontsController(IMapper mapper, IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _webHostEnvironment = webHostEnvironment;
        }

        [HttpGet]
        public IActionResult Get(int? fontId, string? searchString, int pageNumber = 1, int pageSize = 10)
        {
            var fonts = _unitOfWork.FontRepository.GetAll(fontId, searchString, pageNumber, pageSize);
            fonts ??= Enumerable.Empty<Font>();

            return Ok(fonts);
        }

        [HttpPost]
        public IActionResult Add([FromForm] FontCreateDTO fontCreateDTO)
        {
            try
            {
                var path = $"{_webHostEnvironment.WebRootPath}/{SD.FontPath}";
                _unitOfWork.FontRepository.Add(fontCreateDTO, path);
                _unitOfWork.Save();
                return CreatedAtAction(nameof(Get), fontCreateDTO.File.FileName);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
