using Microsoft.EntityFrameworkCore;
using SmartMenu.Domain.Models;
using SmartMenu.Domain.Repository;

namespace SmartMenu.DAO.Implementation
{
    public class BoxItemRepository : GenericRepository<BoxItem>, IBoxItemRepository
    {
        private readonly SmartMenuDBContext _context;

        public BoxItemRepository(SmartMenuDBContext context) : base(context)
        {
            _context = context;
        }

        public IEnumerable<BoxItem> GetAll(int? boxItemId, int? boxId, int? fontId, string? searchString, int pageNumber, int pageSize)
        {
            var data = _context.BoxItems
                .Include(c => c.BFont)
                .AsQueryable();

            return DataQuery(data, boxItemId, boxId, fontId, searchString, pageNumber, pageSize);
        }

        private IEnumerable<BoxItem> DataQuery(IQueryable<BoxItem> data, int? boxItemId, int? boxId, int? fontId, string? searchString, int pageNumber, int pageSize)
        {
            data = data.Where(c => c.IsDeleted == false);

            if (boxItemId != null)
            {
                data = data.Where(c => c.BoxItemId == boxItemId);
            }

            if (boxId != null)
            {
                data = data.Where(c => c.BoxId == boxId);
            }

            if (fontId != null)
            {
                data = data.Where(c => c.BFontId == fontId);
            }

            if (searchString != null)
            {
                data = data.Where(c => c.BoxItemType.ToString() == searchString
                );
            }

            return PaginatedList<BoxItem>.Create(data, pageNumber, pageSize);
        }
    }
}