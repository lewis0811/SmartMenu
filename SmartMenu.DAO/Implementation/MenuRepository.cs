using Microsoft.EntityFrameworkCore;
using SmartMenu.Domain.Models;
using SmartMenu.Domain.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartMenu.DAO.Implementation
{
    public class MenuRepository : GenericRepository<Menu>, IMenuRepository
    {
        private readonly SmartMenuDBContext _context;

        public MenuRepository(SmartMenuDBContext context) : base(context)
        {
            _context = context;
        }

        public IEnumerable<Menu> GetAll(int? menuId, int? brandId, string? searchString, int pageNumber, int pageSize)
        {
            var data = _context.Menus.AsQueryable();

            return DataQuery(data, menuId, brandId, searchString, pageNumber, pageSize);
        }

        public IEnumerable<Menu> GetMenuWithProductGroup(int? menuId, int? brandId, string? searchString, int pageNumber, int pageSize)
        {
            var data = _context.Menus
                .Include(x => x.ProductGroups)
                .AsQueryable();

            return DataQuery(data, menuId, brandId, searchString, pageNumber, pageSize);
        }
        private IEnumerable<Menu> DataQuery(IQueryable<Menu> data, int? menuId, int? brandId, string? searchString, int pageNumber, int pageSize)
        {
            data = data.Where(c => c.IsDeleted == false);
            if (menuId != null)
            {
                data = data
                    .Where(c => c.MenuID == menuId);
            }

            if (brandId != null)
            {
                data = data
                    .Where(c => c.BrandID == brandId);
            }

            if (searchString != null)
            {
                searchString = searchString.Trim();
                data = data
                    .Where(c => c.MenuDescription.Contains(searchString)
                    || c.MenuName.Contains(searchString));
            }

            return PaginatedList<Menu>.Create(data, pageNumber, pageSize);
        }
    }
}
