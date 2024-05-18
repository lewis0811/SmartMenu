using SmartMenu.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartMenu.Domain.Repository
{
    public interface ICategoryRepository : IGenericRepository<Category>
    {
        public IEnumerable<Category> GetAll(int? categoryId, string? searchString, int pageNumber = 1, int pageSize = 10);

    }
}
