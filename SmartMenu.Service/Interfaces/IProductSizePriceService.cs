using SmartMenu.Domain.Models;
using SmartMenu.Domain.Models.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartMenu.Service.Interfaces
{
    public interface IProductSizePriceService
    {
        IEnumerable<ProductSizePrice> GetAll(int? productSizePriceId, int? productId, string? searchString, int pageNumber, int pageSize);
        Task<ProductSizePrice> AddAsync(ProductSizePriceCreateDTO productSizePriceCreateDTO);
        Task<ProductSizePrice> Update(int productSizePriceId, ProductSizePriceUpdateDTO productSizePriceUpdateDTO);
        void Delete(int productSizePriceId);
    }
}
