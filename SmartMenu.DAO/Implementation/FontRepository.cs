using SmartMenu.Domain.Models;
using SmartMenu.Domain.Models.DTO;
using SmartMenu.Domain.Repository;
using System.Drawing.Text;

namespace SmartMenu.DAO.Implementation
{
    public class FontRepository : GenericRepository<BFont>, IFontRepository
    {
        private readonly SmartMenuDBContext _context;

        public FontRepository(SmartMenuDBContext context) : base(context)
        {
            _context = context;
        }

        public IEnumerable<BFont> GetAll(int? fontId, string? searchString, int pageNumber, int pageSize)
        {
            var data = _context.BFonts.AsQueryable();
            return DataQuery(data, fontId, searchString, pageNumber, pageSize);
        }

        public void Add(FontCreateDTO font, string path)
        {
            string fontName = font.File!.FileName;
            //string realfontName = fontName.Split('.').First();
            string extensionName = Path.GetExtension(fontName);

            if (extensionName != ".ttf" && extensionName != ".otf") { throw new Exception("File must be \".ttf\" or \".otf\" extension! "); }

            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            using (FileStream stream = new FileStream(Path.Combine(path, fontName), FileMode.Create))
            {
                font.File.CopyTo(stream);
                stream.Flush();
            }

            PrivateFontCollection fontCollection = new();
            fontCollection.AddFontFile(path + $"\\{fontName}");

            var data = new BFont()
            {
                FontName = fontName,
                FontPath = path + $"\\{fontName}"
            };

            _context.BFonts.Add(data);
            _context.SaveChanges();
        }

        private IEnumerable<BFont> DataQuery(IQueryable<BFont> data, int? fontId, string? searchString, int pageNumber, int pageSize)
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
            return PaginatedList<BFont>.Create(data, pageNumber, pageSize);
        }
    }
}