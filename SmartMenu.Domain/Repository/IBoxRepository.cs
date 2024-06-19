using SmartMenu.Domain.Models;

namespace SmartMenu.Domain.Repository
{
    public interface IBoxRepository : IGenericRepository<Box>
    {
        IEnumerable<Box> GetAll(int? boxId, int? layerId, string? searchString, int pageNumber, int pageSize);
        IEnumerable<Box> GetAllWithBoxItems(int? boxId, int? layerId, string? searchString, int pageNumber, int pageSize);
    }
}
