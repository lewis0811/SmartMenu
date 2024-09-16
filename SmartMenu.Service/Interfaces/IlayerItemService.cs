using SmartMenu.Domain.Models;
using SmartMenu.Domain.Models.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartMenu.Service.Interfaces
{
    public interface ILayerItemService
    {
        IEnumerable<LayerItem> GetAll(int? layerItemId, int? layerId, string? searchString, int pageNumber, int pageSize);
        LayerItem AddLayerItem(LayerItemCreateDTO layerItemCreateDTO);
        LayerItem Update(int layerItemId, LayerItemUpdateDTO layerItemUpdateDTO);
        void Delete(int layerItemId);
    }
}
