#pragma warning disable CA1416 // Validate platform compatibility
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartMenu.API.Ultility;
using SmartMenu.Domain.Models;
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
    [Authorize(Roles = SD.Role_Admin + "," + SD.Role_BrandManager)]
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

        public IActionResult Get(int fondId)
        {
            var font = _unitOfWork.FontRepository.Find(c => c.BFontId == fondId).FirstOrDefault();
            var path = $"{_webHostEnvironment.WebRootPath}\\temp";
            var storePath = Path.Combine(path, Guid.NewGuid().ToString() + ".ttf");

            // Download and write file
#pragma warning disable SYSLIB0014 // Type or member is obsolete
            using (var client = new WebClient())
            {
                client.DownloadFile(font!.FontPath, storePath);
                client.Dispose();
            }
#pragma warning restore SYSLIB0014 // Type or member is obsolete

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
            fonts ??= Enumerable.Empty<BFont>();

            return Ok(fonts);
        }

        [HttpPost]
        [Authorize(Roles = SD.Role_Admin)]
        public async Task<IActionResult> Add([FromForm] FontCreateDTO fontCreateDTO)
        {
            try
            {
                //var path = $"{_webHostEnvironment.WebRootPath}\\{SD.FontPath}";
                string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "fonttemp");
                await _fontService.AddAsync(fontCreateDTO, path);

                return CreatedAtAction(nameof(Get), fontCreateDTO.File!.FileName);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });


            }
        }

        [HttpDelete("{fontId}")]
        [Authorize(Roles = SD.Role_Admin)]
        public IActionResult Delete(int fontId)
        {
            try
            {
                _fontService.Delete(fontId);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }
    }
}
#pragma warning restore CA1416 // Validate platform compatibility