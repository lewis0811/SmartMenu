using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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
        private readonly  IUnitOfWork _unitOfWork;

        public FontsController(IMapper mapper, IUnitOfWork unitOfWork)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        public IActionResult Get(int? fontId, string? searchString, int pageNumber = 1, int pageSize = 10)
        {
            var fonts =  _unitOfWork.FontRepository.GetAll(fontId, searchString, pageNumber, pageSize);
            return Ok(fonts);
        }

        [HttpPost]
        public IActionResult Add(FontCreateDTO fontCreateDTO)
        {
            var data = _mapper.Map<Font>(fontCreateDTO);
            _unitOfWork.FontRepository.Add(data);
            _unitOfWork.Save();
            return CreatedAtAction(nameof(Get), data);
        }
    }
}
