using SmartMenu.Domain.Models;

namespace SmartMenu.Domain.Repository
{
    public interface ILayerRepository : IGenericRepository<Layer>
    {
        IEnumerable<Layer> GetAll(int? layerId, int? templateId, string? searchString, int pageNumber, int pageSize);
        IEnumerable<Layer> GetAllWithBoxes(int? layerId, int? templateId, string? searchString, int pageNumber, int pageSize);
        IEnumerable<Layer> GetAllWithLayerItems(int? layerId, int? templateId, string? searchString, int pageNumber, int pageSize);
        IEnumerable<Layer> GetAllWithLayerItemsAndBoxes(int? layerId, int? templateId, string? searchString, int pageNumber, int pageSize);
    }
}