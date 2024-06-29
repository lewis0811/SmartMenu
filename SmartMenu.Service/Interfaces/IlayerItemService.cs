using SmartMenu.Domain.Models;
using SmartMenu.Domain.Models.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartMenu.Service.Interfaces
{
    public interface IlayerItemService
    {
        LayerItem AddLayerItem(LayerItemCreateDTO layerItemCreateDTO);
        void Delete(int layerItemId);
        IEnumerable<LayerItem> GetAll(int? layerItemId, string? searchString, int pageNumber, int pageSize);
        LayerItem Update(int layerItemId, LayerItemUpdateDTO layerItemUpdateDTO);
    }
}
