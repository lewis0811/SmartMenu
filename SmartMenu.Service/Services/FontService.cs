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

        public IEnumerable<Domain.Models.Font> GetAll(int? fontId, string? searchString, int pageNumber, int pageSize)
        {
            var data = _unitOfWork.FontRepository.EnableQuery();
            var result = DataQuery(data, fontId, searchString, pageNumber, pageSize);
            return result;
        }

        private static IEnumerable<Domain.Models.Font> DataQuery(IQueryable<Domain.Models.Font> data, int? fontId, string? searchString, int pageNumber, int pageSize)
        {
            data = data.Where(c => c.IsDeleted == false);
            if (fontId != null)
            {
                data = data
                    .Where(c => c.FontId == fontId);
            }
            if (searchString != null)
            {
                searchString = searchString.Trim();
                data = data
                    .Where(c => c.FontName.Contains(searchString));
            }
            return PaginatedList<Domain.Models.Font>.Create(data, pageNumber, pageSize);
        }

        public void Add(FontCreateDTO fontCreateDTO, string path)
        {
            PrivateFontCollection fontCollection = new();
            string fontName = fontCreateDTO.File!.FileName;
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

            var data = new Domain.Models.Font()
            {
                FontName = fontCollection.Families.First().Name,
                //FontPath = path + $"\\{fontName}"
                FontPath = uploadResult.SecureUrl.ToString()
            };

            _unitOfWork.FontRepository.Add(data);
            _unitOfWork.Save();
        }
    }
}
