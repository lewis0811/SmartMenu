using SmartMenu.Domain.Models;

namespace SmartMenu.Domain.Repository
{
    public interface IProductRepository : IGenericRepository<Product>
    {
        public IEnumerable<Product> GetAll(int? productId, int? categoryId, string? searchString, int pageNumber = 1, int pageSize = 10);
        /*        public IEnumerable<Product> GetAllProductWithBrand(int? productId, int? brandId, int? categoryId, string? searchString, int pageNumber = 1, int pageSize = 10);
                public IEnumerable<Product> GetAllProductWithCategory(int? productId, int? brandId, int? categoryId, string? searchString, int pageNumber = 1, int pageSize = 10);*/
    }
}
