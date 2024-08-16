using Microsoft.AspNetCore.Mvc;
using System.Drawing;
using System.Drawing.Text;
using System.Net;
using System.Security.Principal;
#pragma warning disable SYSLIB0014 // Type or member is obsolete
#pragma warning disable CA1416 // Validate platform compatibility
namespace SmartMenu.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class IISController : ControllerBase
    {
        private readonly IWebHostEnvironment _webHostEnvironment;

        public IISController(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
        }

        [HttpPost]
        public IActionResult Post()
        {
            try
            {
                // Download and write file
                //var tempPath = $"{_webHostEnvironment.WebRootPath}\\temp";
                var tempPath = "D:\\Temp";
                var tempFontPath = Path.Combine(tempPath, Guid.NewGuid().ToString() + ".ttf");
                using (var client = new WebClient())
                {
                    client.DownloadFile("https://res.cloudinary.com/dchov8fes/raw/upload/v1720345198/fonts/Kollektif.ttf", tempFontPath);
                    client.Dispose();
                }

                // Check if file exists
                if (!System.IO.File.Exists(tempFontPath))
                {
                    throw new FileNotFoundException();
                }

                // Add font to list
                PrivateFontCollection fontCollection = new();
                fontCollection.AddFontFile(tempFontPath);
                var holder = fontCollection.Families[0];
                fontCollection.Dispose();
                Font font = new(holder, 20);

                // Force garbage collection
                GC.Collect();
                GC.WaitForPendingFinalizers(); // Wait for finalizers
                GC.Collect(2, GCCollectionMode.Forced); // More aggressive GC

                // Delete file
                for (int attempt = 0; attempt < 3; attempt++)
                {
                    // Check if the file still exists
                    if (!System.IO.File.Exists(tempFontPath))
                    {
                        break; // File is already deleted, no need to retry
                    }

                    try
                    {
                        System.IO.File.Delete(tempFontPath);
                        break; // Deletion successful, exit loop
                    }
                    catch (Exception ex)
                    {
                        if (attempt < 2) // Retry up to 3 times
                        {
                            Thread.Sleep(100); // Wait for a short time
                        }
                        else
                        {
                            WindowsIdentity currentIdentity = WindowsIdentity.GetCurrent();
                            throw new Exception($"Fail to delete temp font file. Error msg: {ex.Message}\n Current user: {currentIdentity.Name}");
                        }
                    }
                }

                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(new {error = ex.Message });
            }
        }

        [HttpPost("v2")]
        public IActionResult Post2()
        {
            try
            {
                // Download and write file
                //var tempPath = $"{_webHostEnvironment.WebRootPath}\\temp";
                var tempPath = "D:\\Temp";
                var tempFontPath = Path.Combine(tempPath, Guid.NewGuid().ToString() + ".ttf");

                using (var client = new WebClient())
                {
                    client.DownloadFile("https://res.cloudinary.com/dchov8fes/raw/upload/v1720345198/fonts/Kollektif.ttf", tempFontPath);
                    client.Dispose();
                }


                // Check if file exists
                if (!System.IO.File.Exists(tempFontPath))
                {
                    throw new FileNotFoundException();
                }

                // Delete file
                // Delete file with retry logic
                for (int attempt = 0; attempt < 3; attempt++)
                {
                    try
                    {
                        System.IO.File.Delete(tempFontPath);
                        break;
                    }
                    catch (Exception ex)
                    {
                        if (attempt < 2) // Retry up to 3 times
                        {
                            Thread.Sleep(100); // Wait for a short time before retrying
                        }
                        else
                        {
                            // Log the error

                            WindowsIdentity currentIdentity = WindowsIdentity.GetCurrent();

                            throw new Exception($"Fail to delete temp font file. Error msg: {ex.Message}\n Current user: {currentIdentity.Name}");
                        }
                    }
                }
                //StreamWriter SW = new("D:\\Temp\\MyFile.txt");
                //SW.Write("This is a test row!");
                //SW.Close();

                //System.IO.File.Delete("D:\\Temp\\MyFile.txt");
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(new {error = ex.Message });
            }
        }

        [HttpPost("v3")]
        public IActionResult Post3()
        {
            try
            {
                StreamWriter SW = new("D:\\Temp\\MyFile.txt");
                SW.Write("This is a test row!");
                SW.Close();

                System.IO.File.Delete("D:\\Temp\\MyFile.txt");
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(new {error = ex.Message });
            }
        }

        [HttpDelete("temp-folder")]
        public IActionResult ClearTempFolder()
        {
            // 2. Get Temp Folder Path
            string tempFolderPath = "D:\\Temp"; // Update with your actual path

            // 3. Get All Files in the Temp Folder
            string[] files = Directory.GetFiles(tempFolderPath);

            // 4. Delete Each File
            foreach (string file in files)
            {
                try
                {
                    System.IO.File.Delete(file);
                }
                catch (Exception ex) // Handle individual file deletion errors
                {
                    return BadRequest($"Failed to delete file: {tempFolderPath}\n {ex.Message}");
                }
            }

            return Ok("Temp folder cleared successfully.");
        }
    }
}

#pragma warning restore SYSLIB0014 // Type or member is obsolete
#pragma warning restore CA1416 // Validate platform compatibility