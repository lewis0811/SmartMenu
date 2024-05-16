using SmartMenu.Domain.Models;
using SmartMenu.Domain.Repository;

namespace SmartMenu.DAO.Implementation
{
    public class BrandStaffRepository : GenericRepository<BrandStaff>, IBrandStaffRepository
    {
        public BrandStaffRepository(SmartMenuDBContext context) : base(context)
        {
        }
    }
}