using SmartMenu.Domain.Models.DTO;
using SmartMenu.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartMenu.Service.Interfaces
{
    public interface IBrandService
    {
        IEnumerable<Brand> GetAll(int? brandId, string? searchString, int pageNumber, int pageSize);
        IEnumerable<Brand> GetBranchWithStore(int? brandId, string? searchString, int pageNumber, int pageSize);
        IEnumerable<Brand> GetBranchWithBrandStaff(int? brandId, string? searchString, int pageNumber, int pageSize);
        Brand Add(BrandCreateDTO brandCreateDTO);
        Brand Update(int brandId, BrandUpdateDTO brandUpdateDTO);
        void Delete(int brandId);
    }
}
