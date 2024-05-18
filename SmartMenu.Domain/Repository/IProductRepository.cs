using SmartMenu.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartMenu.Domain.Repository
{
    public interface IProductRepository : IGenericRepository<Product>
    {
        public IEnumerable<Product> GetAll(int? productId, int? brandId, int? categoryId, string? searchString, int pageNumber = 1, int pageSize = 10);
/*        public IEnumerable<Product> GetAllProductWithBrand(int? productId, int? brandId, int? categoryId, string? searchString, int pageNumber = 1, int pageSize = 10);
        public IEnumerable<Product> GetAllProductWithCategory(int? productId, int? brandId, int? categoryId, string? searchString, int pageNumber = 1, int pageSize = 10);*/
    }
}
