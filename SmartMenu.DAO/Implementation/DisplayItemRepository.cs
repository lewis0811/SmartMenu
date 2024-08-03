using Microsoft.EntityFrameworkCore;
using SmartMenu.Domain.Models;
using SmartMenu.Domain.Repository;

namespace SmartMenu.DAO.Implementation
{
    public class DisplayItemRepository : GenericRepository<DisplayItem>, IDisplayItemRepository
    {
        private readonly SmartMenuDBContext _context;

        public DisplayItemRepository(SmartMenuDBContext context) : base(context)
        {
            _context = context;
        }

        public IEnumerable<DisplayItem> GetAll(int displayItemId, int displayId, int boxId, int productGroupId, string? searchString, int pageNumber, int pageSize)
        {
            var data = _context.DisplayItems
                .Include(c => c.Box)
                .Include(c => c.ProductGroup)
                .AsQueryable();

            return DataQuery(data, displayItemId, displayId, boxId, productGroupId, searchString, pageNumber, pageSize);
        }

        private IEnumerable<DisplayItem> DataQuery(IQueryable<DisplayItem> data, int displayItemId, int displayId, int boxId, int productGroupId, string? searchString, int pageNumber, int pageSize)
        {
            data = data.Where(c => c.IsDeleted == false);

            data = data.Where(c => c.DisplayId == displayId);

            data = data.Where(c => c.BoxId == boxId);

            data = data.Where(c => c.ProductGroupId == productGroupId);

            if (searchString != null)
            {
                data = data.Where(c => c.Box!.MaxProductItem.ToString().Contains(searchString)
                || c.Box.BoxWidth.ToString().Contains(searchString)
                || c.Box.BoxHeight.ToString().Contains(searchString)
                || c.Box.BoxPositionX.ToString().Contains(searchString)
                || c.Box.BoxPositionY.ToString().Contains(searchString)
                || c.ProductGroup!.ProductGroupName.Contains(searchString));
            }

            return PaginatedList<DisplayItem>.Create(data, pageNumber, pageSize);
        }
    }
}