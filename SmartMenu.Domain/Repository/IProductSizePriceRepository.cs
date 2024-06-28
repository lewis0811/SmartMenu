using SmartMenu.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartMenu.Domain.Repository
{
    public interface IProductSizePriceRepository : IGenericRepository<ProductSizePrice>
    {
        IEnumerable<ProductSizePrice> GetAll(int? productSizePriceId, int? productId, string? searchString, int pageNumber, int pageSize);
    }
}
