using Microsoft.EntityFrameworkCore;
using SmartMenu.Domain.Models;
using SmartMenu.Domain.Repository;

namespace SmartMenu.DAO.Implementation
{
    public class BrandRepository : GenericRepository<Brand>, IBrandRepository
    {
        private readonly SmartMenuDBContext _context;
        public BrandRepository(SmartMenuDBContext context) : base(context)
        {
            _context = context;
        }

        public IEnumerable<Brand> GetAll(int? brandId, string? searchString, int pageNumber = 1, int pageSize = 10)
        {
            var data = _context.Brands.AsQueryable();
            return DataQuery(data, brandId, searchString, pageNumber, pageSize);
        }

        public IEnumerable<Brand> GetBranchWithBrandStaff(int? brandId, string? searchString, int pageNumber = 1, int pageSize = 10)
        {
            var data = _context.Brands.Include(x => x.BrandStaffs).AsQueryable();
            return DataQuery(data, brandId, searchString, pageNumber, pageSize);
        }

        public IEnumerable<Brand> GetBranchWithProduct(int? brandId, string? searchString, int pageNumber = 1, int pageSize = 10)
        {
            var data = _context.Brands.Include(x => x.Products).AsQueryable();
            return DataQuery(data, brandId, searchString, pageNumber, pageSize);
        }

        public IEnumerable<Brand> GetBranchWithStore(int? brandId, string? searchString, int pageNumber = 1, int pageSize = 10)
        {
            var data = _context.Brands.Include(x => x.Stores).AsQueryable();
            return DataQuery(data, brandId, searchString, pageNumber, pageSize);
        }
        
        private IEnumerable<Brand> DataQuery(IQueryable<Brand> data, int? brandId, string? searchString, int pageNumber, int pageSize)
        {
            data = data.Where(c => c.IsDeleted == false);
            if (brandId != null)
            {
                data = data
                    .Where(c => c.BrandID == brandId);
            }

            if (searchString != null)
            {
                searchString = searchString.Trim();
                data = data
                    .Where(c => c.BrandName.Contains(searchString)
                    || c.BrandDescription.Contains(searchString)
                    || c.BrandImage.Contains(searchString)
                    || c.BrandContactEmail.Contains(searchString));
            }

            return PaginatedList<Brand>.Create(data, pageNumber, pageSize);
        }
    }
}
