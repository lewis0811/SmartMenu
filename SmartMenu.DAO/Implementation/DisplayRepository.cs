using Microsoft.EntityFrameworkCore;
using SmartMenu.Domain.Models;
using SmartMenu.Domain.Repository;

namespace SmartMenu.DAO.Implementation
{
    public class DisplayRepository : GenericRepository<Display>, IDisplayRepository
    {
        private readonly SmartMenuDBContext _context;

        public DisplayRepository(SmartMenuDBContext context) : base(context)
        {
            _context = context;
        }

        public IEnumerable<Display> GetAll(int? displayId, int? menuId, int? collectionId, string? searchString, int pageNumber, int pageSize)
        {
            var data = _context.Displays
                .Include(c => c.Menu)
                .Include(c => c.Collection)
                .AsQueryable();

            return DataQuery(data, displayId, menuId, collectionId, searchString, pageNumber, pageSize);
        }

        private IEnumerable<Display> DataQuery(IQueryable<Display> data, int? displayId, int? menuId, int? collectionId, string? searchString, int pageNumber, int pageSize)
        {
            data = data.Where(c => c.IsDeleted == false);

            if (displayId != null)
            {
                data = data.Where(c => c.DisplayId == displayId);
            }

            if (menuId != null)
            {
                data = data.Where(c => c.MenuId == menuId);
            }

            if (collectionId != null)
            {
                data = data.Where(c => c.CollectionId == collectionId);
            }

            if (searchString != null)
            {
                data = data.Where(c => c.StartingHour.ToString().Contains(searchString)
                                                                            || c.EndingHour.ToString()!.Contains(searchString));
            }

            return PaginatedList<Display>.Create(data, pageNumber, pageSize);
        }
    }
}