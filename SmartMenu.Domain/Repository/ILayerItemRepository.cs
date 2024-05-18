using SmartMenu.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartMenu.Domain.Repository
{
    public interface ILayerItemRepository : IGenericRepository<LayerItem>
    {
        IEnumerable<LayerItem> GetAll(int? layerItemId, int? layerId, string? searchString, int pageNumber, int pageSize);
    }
}
