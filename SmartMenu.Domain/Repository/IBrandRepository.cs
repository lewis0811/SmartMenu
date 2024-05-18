using SmartMenu.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartMenu.Domain.Repository
{
    public interface IBrandRepository : IGenericRepository<Brand>
    {
        public IEnumerable<Brand> GetAll(int? brandId, string? searchString, int pageNumber = 1, int pageSize = 10);

        public IEnumerable<Brand> GetBranchWithBrandStaff(int? brandId, string? searchString, int pageNumber = 1, int pageSize = 10);

        public IEnumerable<Brand> GetBranchWithStore(int? brandId, string? searchString, int pageNumber = 1, int pageSize = 10);
        public IEnumerable<Brand> GetBranchWithProduct( int? brandId, string? searchString, int pageNumber = 1, int pageSize = 10);
    }
}
