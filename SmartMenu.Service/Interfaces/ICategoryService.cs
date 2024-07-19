using SmartMenu.Domain.Models;
using SmartMenu.Domain.Models.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartMenu.Service.Interfaces
{
    public interface ICategoryService
    {
        public IEnumerable<Category> GetAll(int? categoryId, int? brandId, string? searchString, int pageNumber, int pageSize);
        Category Add(CategoryCreateDTO categoryCreateDTO);
        Category Update(int categoryId, CategoryCreateDTO categoryCreateDTO);
        void Delete(int categoryId);
    }
}
