using SmartMenu.Domain.Models;
using SmartMenu.Domain.Repository;

namespace SmartMenu.DAO.Implementation
{
    public class BrandStaffRepository : GenericRepository<BrandStaff>, IBrandStaffRepository
    {
        private readonly SmartMenuDBContext _context;

        public BrandStaffRepository(SmartMenuDBContext context) : base(context)
        {
            _context = context;
        }

        public IEnumerable<BrandStaff> GetAll(int? brandStaffId, int? brandId, Guid? userId, string? searchString, int pageNumber = 1, int pageSize = 10)
        {
            var data = _context.BrandStaffs.AsQueryable();

            return DataQuery(data, brandStaffId, brandId, userId, searchString, pageNumber, pageSize);
        }
        private IEnumerable<BrandStaff> DataQuery(IQueryable<BrandStaff> data, int? brandStaffId, int? brandId, Guid? userId, string? searchString, int pageNumber, int pageSize)
        {
            data = data.Where(c => c.IsDeleted == false);
            if (brandStaffId != null)
            {
                data = data
                    .Where(c => c.BrandStaffId == brandStaffId);
            }

            if (brandId != null)
            {
                data = data
                    .Where(c => c.BrandId == brandId);
            }
            if (userId != null)
            {
                data = data
                    .Where(c => c.UserId == userId);
            }


            return PaginatedList<BrandStaff>.Create(data, pageNumber, pageSize);
        }
    }
}