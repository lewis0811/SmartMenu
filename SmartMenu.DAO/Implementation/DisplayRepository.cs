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

        public Display GetWithItems(int displayId)
        {
            var data = _context.Displays
                .Include(c => c.DisplayItems)!
                .ThenInclude(c => c.ProductGroup)!
                .ThenInclude(c => c!.ProductGroupItems)!
                .ThenInclude(c => c!.Product)
                .ThenInclude(c => c!.ProductSizePrices)
                .Include(c => c.DisplayItems)!
                .ThenInclude(c => c.Box)
                .ThenInclude(c => c!.BoxItems)
                .Where(c => c.DisplayId == displayId)
                .FirstOrDefault();

            return data!;
        }

        public IEnumerable<Display> GetWithItems(int? displayId, int? menuId, int? collectionId, string? searchString, int pageNumber, int pageSize)
        {
            var data = _context.Displays
                .Include(c => c.DisplayItems)!
                .ThenInclude(c => c.ProductGroup)!
                .ThenInclude(c => c!.ProductGroupItems)!
                .ThenInclude(c => c!.Product)
                .ThenInclude(c => c!.ProductSizePrices)
                .Include(c => c.DisplayItems)!
                .ThenInclude(c => c.Box)
                .ThenInclude(c => c!.BoxItems);

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
                data = data.Where(c => c.ActiveHour.ToString().Contains(searchString));
            }

            return PaginatedList<Display>.Create(data, pageNumber, pageSize);
        }
    }
}