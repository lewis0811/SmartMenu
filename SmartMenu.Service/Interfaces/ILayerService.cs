using SmartMenu.Domain.Models;
using SmartMenu.Domain.Models.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartMenu.Service.Interfaces
{
    public interface ILayerService
    {
        Task<Domain.Models.Layer> AddLayerAsync(LayerCreateDTO layerCreateDTO);
        void Delete(int layerId);
        IEnumerable<Layer> GetAll(int? layerId, int? templateId, string? searchString, int pageNumber, int pageSize);
        IEnumerable<Layer> GetAllWithBoxes(int? layerId, int? templateId, string? searchString, int pageNumber, int pageSize);
        IEnumerable<Layer> GetAllWithLayerItems(int? layerId, int? templateId, string? searchString, int pageNumber, int pageSize);
        IEnumerable<Layer> GetAllWithLayerItemsAndBoxes(int? layerId, int? templateId, string? searchString, int pageNumber, int pageSize);
        Layer Update(int layerId, LayerUpdateDTO layerUpdateDTO);
    }
}
