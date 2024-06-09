using SmartMenu.Domain.Models;

namespace SmartMenu.Domain.Repository
{
    public interface IMenuRepository : IGenericRepository<Menu>
    {
        public IEnumerable<Menu> GetAll(int? menuId, int? brandId, string? searchString, int pageNumber = 1, int pageSize = 10);

        public IEnumerable<Menu> GetMenuWithProductGroup(int? menuId, int? brandId, string? searchString, int pageNumber = 1, int pageSize = 10);
    }
}
