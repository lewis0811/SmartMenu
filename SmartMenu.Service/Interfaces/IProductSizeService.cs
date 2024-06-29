using SmartMenu.Domain.Models;
using SmartMenu.Domain.Models.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartMenu.Service.Interfaces
{
    public interface IProductSizeService
    {
        IEnumerable<ProductSize> GetAll(int? productSizeId, string? searchString, int pageNumber, int pageSize);
        ProductSize Add(ProductSizeCreateDTO productSizeCreateDTO);
        ProductSize Update(int productSizeId, ProductSizeCreateDTO productSizeCreateDTO);
        void Delete(int productSizeId);
    }
}
