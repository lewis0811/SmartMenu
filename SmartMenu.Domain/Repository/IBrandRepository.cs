using SmartMenu.Domain.Models;

namespace SmartMenu.Domain.Repository
{
    public interface IBrandRepository : IGenericRepository<Brand>
    {
        public IEnumerable<Brand> GetAll(int? brandId, string? searchString, int pageNumber = 1, int pageSize = 10);

        public IEnumerable<Brand> GetBranchWithBrandStaff(int? brandId, string? searchString, int pageNumber = 1, int pageSize = 10);

        public IEnumerable<Brand> GetBranchWithStore(int? brandId, string? searchString, int pageNumber = 1, int pageSize = 10);
    }
}
