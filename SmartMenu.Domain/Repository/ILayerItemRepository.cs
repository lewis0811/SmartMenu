using SmartMenu.Domain.Models;

namespace SmartMenu.Domain.Repository
{
    public interface ILayerItemRepository : IGenericRepository<LayerItem>
    {
        IEnumerable<LayerItem> GetAll(int? layerItemId, string? searchString, int pageNumber, int pageSize);
    }
}
