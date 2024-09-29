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
            _cloudinary = new(configuration.GetSection("Cloudinary:CLOUDINARY_URL").Value);
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

        public async Task AddAsync(FontCreateDTO fontCreateDTO, string path)
        {
            string fontName = fontCreateDTO.File!.FileName;
            string fontNameWithoutExtension = Path.GetFileNameWithoutExtension(fontName);
            string extensionName = Path.GetExtension(fontName);

            if (extensionName != ".ttf" && extensionName != ".otf")
            {
                throw new ArgumentException("File must be \".ttf\" or \".otf\" extension!");
            }

            var existingFont = await _unitOfWork.FontRepository.FindObjectAsync(c => c.FontName == fontNameWithoutExtension);
            if (existingFont != null)
            {
                throw new InvalidOperationException($"Font: `{fontNameWithoutExtension}` already exists");
            }

            Directory.CreateDirectory(path);
            string fullPath = Path.Combine(path, fontName);

            await using (var stream = new FileStream(fullPath, FileMode.Create))
            {
                await fontCreateDTO.File.CopyToAsync(stream);
            }

            FontFamily family;
            using (var fontCollection = new PrivateFontCollection())
            {
                fontCollection.AddFontFile(fullPath);
                family = fontCollection.Families.FirstOrDefault(f => f.Name.Equals(fontCollection.Families[0].Name, StringComparison.OrdinalIgnoreCase))
                    ?? throw new FileNotFoundException("Font not found!");

                if (!family.IsStyleAvailable(FontStyle.Regular))
                {
                    throw new InvalidOperationException("Font doesn't support regular style, please add another font");
                }
            }

            var uploadParams = new RawUploadParams
            {
                File = new FileDescription(fullPath),
                Folder = "fonts",
                PublicId = fontNameWithoutExtension,
            };

            var uploadResult = await _cloudinary.UploadAsync(uploadParams);
            if (uploadResult.Error != null)
            {
                throw new Exception($"Upload failed: {uploadResult.Error.Message}");
            }

            var font = new Domain.Models.BFont
            {
                FontName = fontNameWithoutExtension,
                FontPath = uploadResult.SecureUrl.ToString()
            };

            _unitOfWork.FontRepository.Add(font);
            _unitOfWork.Save();


        }

        public void Delete(int fontId)
        {
            var font = _unitOfWork.FontRepository.Find(c => c.BFontId == fontId).FirstOrDefault()
                ?? throw new Exception("Font not found or deleted");
            if (font != null)
            {
                _unitOfWork.FontRepository.Remove(font);
                _unitOfWork.Save();
            }
        }
    }
}
#pragma warning restore CA1416 // Validate platform compatibility