using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using SmartMenu.Domain.Models.DTO;
using SmartMenu.Domain.Repository;
using SmartMenu.Service.Interfaces;
using System.Drawing;
using System.Drawing.Text;
using System.Net;

namespace SmartMenu.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FontsController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IFontService _fontService;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public FontsController(IMapper mapper, IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment, IFontService fontService)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _webHostEnvironment = webHostEnvironment;
            _fontService = fontService;
        }

        [HttpGet("test")]
        public IActionResult Get(string imgpath, int fondId)
        {
            var font = _unitOfWork.FontRepository.Find(c => c.FontId == fondId).FirstOrDefault();
            var path = $"{_webHostEnvironment.WebRootPath}\\temp";
            var storePath = Path.Combine(path, Guid.NewGuid().ToString() + ".ttf");

            // Download and write file
            using (var client = new WebClient())
            {
                client.DownloadFile(font.FontPath, storePath);
                client.Dispose();
            }

            // Check if file exists
            if (!System.IO.File.Exists(storePath))
            {
                throw new FileNotFoundException();
            }

            // Initialize font through font collection
            PrivateFontCollection fontCollection = new();
            fontCollection.AddFontFile(storePath);

            // Find the specified font family
            FontFamily family = fontCollection.Families.FirstOrDefault(f => f.Name.Equals(fontCollection.Families[0].Name, StringComparison.OrdinalIgnoreCase))
                ?? throw new Exception("Font not found!");

            // Check if the font family was found and supports regular style
            bool isSupport = family.IsStyleAvailable(FontStyle.Regular);
            if (!isSupport) { throw new Exception("Font doesn't support regular style, please add another font"); }

            System.Drawing.Font returnFont = new(fontCollection.Families[0], 20);
            if (returnFont != null)
            {
                System.IO.File.Delete(storePath);
            }

            return Ok(returnFont);
        }

        [HttpGet]
        public IActionResult Get(int? fontId, string? searchString, int pageNumber = 1, int pageSize = 10)
        {
            var fonts = _fontService.GetAll(fontId, searchString, pageNumber, pageSize);
            fonts ??= Enumerable.Empty<Domain.Models.Font>();

            return Ok(fonts);
        }

        [HttpPost]
        public IActionResult Add([FromForm] FontCreateDTO fontCreateDTO)
        {
            try
            {
                //var path = $"{_webHostEnvironment.WebRootPath}\\{SD.FontPath}";
                var path = $"{_webHostEnvironment.WebRootPath}\\temp";
                _fontService.Add(fontCreateDTO, path);

                return CreatedAtAction(nameof(Get), fontCreateDTO.File!.FileName);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}