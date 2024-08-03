using SmartMenu.Domain.Models;
using System;
using System.Collections.Generic;
using System.Drawing.Text;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
#pragma warning disable CA1416 // Validate platform compatibility
#pragma warning disable SYSLIB0014 // Type or member is obsolete
namespace SmartMenu.Service
{
    public static class Ultilities
    {
        private static readonly string TempFontDirectory = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "temp");
        public static readonly Dictionary<int, FontFamily> FontFamilies = new();

        public static Font InitializeFont(float fontSize, FontStyle fontStyle, BFont bFont)
        {
            // 0. Check if FontFamily already exists
            FontFamily fontFamily;
            if (FontFamilies.TryGetValue(bFont.BFontId, out fontFamily!))
            {
                // Font exists, return new Font with the specified style and size
                return new Font(fontFamily, fontSize, fontStyle);
            }

            // 1. Create the temp directory if it doesn't exist
            Directory.CreateDirectory(TempFontDirectory);

            // 2. Generate unique font path in the temp directory
            string tempFontPath = Path.Combine(TempFontDirectory, $"{Guid.NewGuid()}.ttf");

            // 3. Download font with error handling
            try
            {

                using WebClient client = new();

                client.DownloadFile(bFont.FontPath!, tempFontPath);
            }
            catch (WebException ex)
            {
                throw new InvalidOperationException("Error downloading font: " + ex.Message);
            }

            // 4. Add font to private collection and store FontFamily
            using (var fontCollection = new PrivateFontCollection())
            {
                try
                {
                    var fontByte = File.ReadAllBytes(tempFontPath);
                    var pinned = GCHandle.Alloc(fontByte, GCHandleType.Pinned);
                    var pointer = pinned.AddrOfPinnedObject();

                    fontCollection.AddMemoryFont(pointer, fontByte.Length);

                    fontFamily = fontCollection.Families[0];  // Get the FontFamily

                    pinned.Free(); // Always free the handle
                }
                catch (ArgumentException ex)
                {
                    throw new InvalidOperationException("Invalid font file: " + ex.Message);
                }
                finally
                {
                }
            }

            // 5. Add FontFamily to dictionary and create the Font object
            FontFamilies.Add(bFont.BFontId, fontFamily);
            return new Font(fontFamily, fontSize, fontStyle);
        }
    }
}

#pragma warning restore SYSLIB0014 // Type or member is obsolete
#pragma warning restore CA1416 // Validate platform compatibility   