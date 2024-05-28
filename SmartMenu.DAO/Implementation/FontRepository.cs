using SmartMenu.Domain.Models;
using SmartMenu.Domain.Models.DTO;
using SmartMenu.Domain.Repository;

namespace SmartMenu.DAO.Implementation
{
    public class FontRepository : GenericRepository<Font>, IFontRepository
    {
        private readonly SmartMenuDBContext _context;


        public FontRepository(SmartMenuDBContext context) : base(context)
        {
            _context = context;
        }

        public IEnumerable<Font> GetAll(int? fontId, string searchString, int pageNumber, int pageSize)
        {
            var data = _context.Fonts.AsQueryable();
            return DataQuery(data, fontId, searchString, pageNumber, pageSize);
        }

        public void Add (FontCreateDTO font)
        {
            string fontName = Path.GetFileName(font.File.FileName);
        }

        private IEnumerable<Font> DataQuery(IQueryable<Font> data, int? fontId, string? searchString, int pageNumber, int pageSize)
        {
            data = data.Where(c => c.IsDeleted == false);
            if (fontId != null)
            {
                data = data
                    .Where(c => c.FontID == fontId);
            }
            if (searchString != null)
            {
                searchString = searchString.Trim();
                data = data
                    .Where(c => c.FontName.Contains(searchString));
            }
            return PaginatedList<Font>.Create(data, pageNumber, pageSize);
        }
    }
}