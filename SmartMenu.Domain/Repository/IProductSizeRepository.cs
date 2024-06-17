using SmartMenu.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartMenu.Domain.Repository
{
    public interface IProductSizeRepository : IGenericRepository<ProductSize>
    {
        IEnumerable<ProductSize> GetAll(int? productSizeId, string? searchString, int pageNumber, int pageSize);
    }
}
