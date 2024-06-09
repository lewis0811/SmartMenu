using SmartMenu.Domain.Models;

namespace SmartMenu.Domain.Repository
{
    public interface IBoxRepository : IGenericRepository<Box>
    {
        IEnumerable<Box> GetAll(int? boxId, int? layerId, int? fontId, string? searchString, int pageNumber, int pageSize);
    }
}
