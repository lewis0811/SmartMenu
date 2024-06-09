using SmartMenu.Domain.Models;

namespace SmartMenu.Domain.Repository
{
    public interface IBrandStaffRepository : IGenericRepository<BrandStaff>
    {
        public IEnumerable<BrandStaff> GetAll(int? brandStaffId, int? brandId, Guid? userId, string? searchString, int pageNumber = 1, int pageSize = 10);
    }
}
