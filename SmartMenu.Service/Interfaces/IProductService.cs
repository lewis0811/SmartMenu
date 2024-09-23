using SmartMenu.Domain.Models;
using SmartMenu.Domain.Models.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartMenu.Service.Interfaces
{
    public interface IProductService
    {
        IEnumerable<Product> GetAll(int? productId, int? categoryId, string? searchString, int pageNumber, int pageSize);
        Product Add(ProductCreateDTO productCreateDTO);
        Product Update(int productId, ProductUpdateDTO productUpdateDTO);
        void Delete(int productId);
        IEnumerable<Product> GetProductByMenuOrCollection(int? menuId, int? collectionId);
        IEnumerable<Product> GetProductByCollection(int collectionId);
        IEnumerable<Product> GetByBrand(int brandId, string? searchString, int pageNumber, int pageSize);
    }
}
