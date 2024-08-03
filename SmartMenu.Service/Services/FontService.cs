using CloudinaryDotNet;
using Microsoft.EntityFrameworkCore;
using SmartMenu.DAO;
using SmartMenu.Domain.Models;
using SmartMenu.Domain.Models.DTO;
using SmartMenu.Domain.Repository;
using SmartMenu.Service.Interfaces;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing.Text;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CloudinaryDotNet.Actions;
using Microsoft.Extensions.Configuration;
#pragma warning disable CA1416 // Validate platform compatibility

namespace SmartMenu.Service.Services
{
    public class FontService : IFontService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly Cloudinary _cloudinary;

        public FontService(IUnitOfWork unitOfWork, IConfiguration configuration)
        {
            _unitOfWork = unitOfWork;
            _cloudinary = new (configuration.GetSection("Cloudinary:CLOUDINARY_URL").Value);
        }

        public IEnumerable<Domain.Models.BFont> GetAll(int? fontId, string? searchString, int pageNumber, int pageSize)
        {
            var data = _unitOfWork.FontRepository.EnableQuery();
            var result = DataQuery(data, fontId, searchString, pageNumber, pageSize);
            return result;
        }

        private static IEnumerable<Domain.Models.BFont> DataQuery(IQueryable<Domain.Models.BFont> data, int? fontId, string? searchString, int pageNumber, int pageSize)
        {
            data = data.Where(c => c.IsDeleted == false);
            if (fontId != null)
            {
                data = data
                    .Where(c => c.BFontId == fontId);
            }
            if (searchString != null)
            {
                searchString = searchString.Trim();
                data = data
                    .Where(c => c.FontName.Contains(searchString));
            }
            return PaginatedList<Domain.Models.BFont>.Create(data, pageNumber, pageSize);
        }

        public void Add(FontCreateDTO fontCreateDTO, string path)
        {

            PrivateFontCollection fontCollection = new();
            string fontName = fontCreateDTO.File!.FileName;
            string fontNameCheck = fontName.Split('.')[0];

            var existFont = _unitOfWork.FontRepository.Find(c => c.FontName == fontNameCheck).FirstOrDefault();
            if (existFont != null) throw new Exception($"Font: `{fontNameCheck}` already exist ");

            //string realfontName = fontName.Split('.').First();
            string extensionName = Path.GetExtension(fontName);

            if (extensionName != ".ttf" && extensionName != ".otf") { throw new Exception("File must be \".ttf\" or \".otf\" extension! "); }

            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            using (FileStream stream = new(Path.Combine(path, fontName), FileMode.Create))
            {
                fontCreateDTO.File.CopyTo(stream);
                stream.Flush();
            }

            // Add to font collection
            fontCollection.AddFontFile(Path.Combine(path, fontName));

            // Find the specified font family

            FontFamily family = fontCollection.Families.FirstOrDefault(f => f.Name.Equals(fontCollection.Families[0].Name, StringComparison.OrdinalIgnoreCase))
                ?? throw new Exception("Font not found!");


            // Check if the font family was found and supports regular style
            bool isSupport = family.IsStyleAvailable(FontStyle.Regular);
            if (!isSupport) { throw new Exception("Font doesn't support regular style, please add another font"); }

            // Upload Parameters
            var uploadParams = new RawUploadParams()
            {
                File = new FileDescription(Path.Combine(path, fontName)), // Specify the font file
                Folder = "fonts",                         // Optional: Organize fonts in a folder
                PublicId = Path.GetFileNameWithoutExtension(Path.Combine(path, fontName)),  // Use file name as Public ID
            };

            RawUploadResult uploadResult = _cloudinary.Upload(uploadParams);
            if (uploadResult.Error != null)
            {
                throw new Exception($"Upload failed: {uploadResult.Error.Message}");
            }


            fontCollection.AddFontFile(path + $"\\{fontName}");

            var data = new Domain.Models.BFont()
            {
                FontName = fontName.Split('.')[0],
                //FontPath = path + $"\\{fontName}"
                FontPath = uploadResult.SecureUrl.ToString()
            };

            _unitOfWork.FontRepository.Add(data);
            _unitOfWork.Save();
        }
    }
}
#pragma warning restore CA1416 // Validate platform compatibility