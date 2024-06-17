using SmartMenu.Domain.Models;

namespace SmartMenu.Domain.Repository
{
    public interface IBoxItemRepository : IGenericRepository<BoxItem>
    {
        IEnumerable<BoxItem> GetAll(int? boxItemId, int? boxId, int? fontId, string? searchString, int pageNumber, int pageSize);
    }
}