using SmartMenu.Domain.Models;
using SmartMenu.Domain.Models.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartMenu.Service.Interfaces
{
    public interface IBrandStaffService
    {
        IEnumerable<BrandStaff> GetAll(int? brandStaffId, int? brandId, Guid? userId, string? searchString, int pageNumber, int pageSize);
        BrandStaff Add(BrandStaffCreateDTO brandStaffCreateDTO);
        BrandStaff Update(int brandStaffId, BrandStaffCreateDTO brandStaffCreateDTO);
        void Delete(int brandStaffId);
    }
}
