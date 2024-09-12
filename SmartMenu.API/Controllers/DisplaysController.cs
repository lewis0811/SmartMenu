using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartMenu.DAO;
using SmartMenu.Domain.Models;
using SmartMenu.Domain.Models.DTO;
using SmartMenu.Service.Interfaces;
using System;
using System.Diagnostics;

namespace SmartMenu.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DisplaysController : ControllerBase
    {
        private readonly IDisplayService _displayService;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly SmartMenuDBContext _context;

        public DisplaysController(IDisplayService displayService, IWebHostEnvironment webHostEnvironment, SmartMenuDBContext context)
        {
            _displayService = displayService;
            _webHostEnvironment = webHostEnvironment;
            _context = context;
        }

        [HttpGet("text")]
        public async Task<IActionResult> Test(int displayId)
        {
            var data = _context.Displays.FirstOrDefault(c => c.DisplayId == displayId);
            return Ok(data);
        }

        [HttpGet]
        public IActionResult Get(int? displayId, int? menuId, int? collectionId, string? searchString, int pageNumber = 1, int pageSize = 10)
        {
            try
            {
                var data = _displayService.GetAll(displayId, menuId, collectionId, searchString, pageNumber, pageSize);
                return Ok(data);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpGet("DisplayItems")]
        public IActionResult GetWithItems(int? displayId, int? menuId, int? collectionId, string? searchString, int pageNumber = 1, int pageSize = 10)
        {
            try
            {
                var data = _displayService.GetWithItems(displayId, menuId, collectionId, searchString, pageNumber, pageSize);
                return Ok(data);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpGet("DisplayItems/v2")]
        public IActionResult GetWithItemsV2(int storeId, int? deviceId, int? displayId, int? menuId, int? collectionId, string? searchString, int pageNumber = 1, int pageSize = 10)
        {
            try
            {
                var data = _displayService.GetWithItemsV2(storeId, deviceId, displayId, menuId, collectionId, searchString, pageNumber, pageSize);
                return Ok(data);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpGet("{deviceId}")]
        public async Task<IActionResult> GetByDeviceId(int? deviceId)
        {
            try
            {
                var data = await _displayService.GetByDeviceId(deviceId);
                return Ok(data);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
                throw;
            }
        }

        [HttpGet("{displayId}/template/image")]
        public async Task<IActionResult> GetTemplateImageAsync(int displayId)
        {
            try
            {
                var data = await _displayService.GetTemplateImageAsync(displayId);
                if (data == null) return BadRequest("Image fail to create");
                _displayService.DeleteTempFile();

                var isUri = Uri.IsWellFormedUriString(data, UriKind.RelativeOrAbsolute);
                if (isUri) { return Redirect(data); }

                byte[] b = System.IO.File.ReadAllBytes(data);
                return File(b, "image/jpeg");
            }
            catch (Exception ex)
            {
                return BadRequest($"{ex.Message}");
            }
        }

        [HttpGet("V1/{deviceId}/image")]
        public async Task<IActionResult> GetImageAsync(int deviceId)
        {
            try
            {
                var data = await _displayService.GetImageByTimeAsync(deviceId);
                if (data == null) return BadRequest("Image fail to create");
                _displayService.DeleteTempFile();

                var isUri = Uri.TryCreate(data, UriKind.RelativeOrAbsolute, out Uri? uri);
                if (isUri) { return Redirect(data); }

                byte[] b = System.IO.File.ReadAllBytes(data);
                return File(b, "image/jpeg");
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpGet("V2/{displayId}/image")]
        public async Task<IActionResult> GetImageV2Async(int displayId)
        {
            try
            {
                var data = await _displayService.GetImageByDisplayAsync(displayId);
                if (data == null) return BadRequest("Image fail to create");
                _displayService.DeleteTempFile();

                var isUri = Uri.IsWellFormedUriString(data, UriKind.RelativeOrAbsolute);
                if (isUri) { return Redirect(data); }

                byte[] b = System.IO.File.ReadAllBytes(data);
                return File(b, "image/jpeg");
            }
            catch (Exception ex)
            {
                return BadRequest($"{ex.Message}");
            }
        }


        [HttpPost]
        public IActionResult Add(DisplayCreateDTO displayCreateDTO)
        {
            try
            {
                var data = _displayService.AddDisplay(displayCreateDTO);
                return CreatedAtAction(nameof(Get), data);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpPost("V2")]
        public async Task<IActionResult> AddV2Async(DisplayCreateDTO displayCreateDTO)
        {
            try
            {
                var data = await _displayService.AddDisplayV2Async(displayCreateDTO);
                _displayService.DeleteTempFile();

                return CreatedAtAction(nameof(Get), $"Display Id: {data.DisplayId} initialized successfully");
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpPut("{displayId}")]
        public IActionResult Update(int displayId, DisplayUpdateDTO displayUpdateDTO)
        {
            try
            {
                var data = _displayService.Update(displayId, displayUpdateDTO);
                _displayService.DeleteTempFile();

                return Ok(data);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpDelete("{displayId}")]
        public IActionResult Delete(int displayId)
        {
            try
            {
                _displayService.Delete(displayId);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpDelete("temp-files")]
        public IActionResult ClearTempFolder()
        {
            // 2. Get Temp Folder Path
            var tempPath = $"{_webHostEnvironment.WebRootPath}\\temp";

            // Check if folder exist
            if (!Directory.Exists(tempPath))
            {
                Directory.CreateDirectory(tempPath);
            }

            // 3. Get All Files in the Temp Folder
            string[] files = Directory.GetFiles(tempPath);

            // 4. Delete Each File
            foreach (string file in files)
            {
                try
                {
                    System.IO.File.Delete(file);
                }
                catch (Exception ex) // Handle individual file deletion errors
                {
                    return BadRequest($"Failed to delete file: {tempPath}\n {ex.Message}");
                }
            }

            return Ok("Temp folder cleared successfully.");
        }
    }
}