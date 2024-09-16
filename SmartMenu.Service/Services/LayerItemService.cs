using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SmartMenu.DAO;
using SmartMenu.Domain.Models;
using SmartMenu.Domain.Models.DTO;
using SmartMenu.Domain.Models.Enum;
using SmartMenu.Domain.Repository;
using SmartMenu.Service.Interfaces;

namespace SmartMenu.Service.Services
{
    public class LayerItemService : ILayerItemService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public LayerItemService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public LayerItem AddLayerItem(LayerItemCreateDTO layerItemCreateDTO)
        {
            var layer = _unitOfWork.LayerRepository.Find(c => c.LayerId == layerItemCreateDTO.LayerID && c.IsDeleted == false).FirstOrDefault()
            ?? throw new Exception("Layer not found or deleted");

            var existedLayerItem = _unitOfWork.LayerItemRepository.Find(c => c.LayerId == layer.LayerId && c.IsDeleted == false).FirstOrDefault();
            if (existedLayerItem != null) throw new Exception($"Layer ID: {layer.LayerId} already have layer item");

            if (layer.LayerType == LayerType.Content) throw new Exception("Render layer can't have layer item");
            //if (layer.LayerType == LayerType.MenuCollectionNameLayer) throw new Exception("MenuCollectionName layer can't have layer item");


            var data = _mapper.Map<LayerItem>(layerItemCreateDTO);

            _unitOfWork.LayerItemRepository.Add(data);
            _unitOfWork.Save();

            UpdateDisplayIfExist(data);

            return data;
        }

        public void Delete(int layerItemId)
        {
            var data = _unitOfWork.LayerItemRepository.Find(c => c.LayerItemId == layerItemId && c.IsDeleted == false).FirstOrDefault()
            ?? throw new Exception("Layer item not found or deleted");

            //data.IsDeleted = true;

            _unitOfWork.LayerItemRepository.Remove(data);
            _unitOfWork.Save();

            UpdateDisplayIfExist(data);
        }

        public IEnumerable<LayerItem> GetAll(int? layerItemId, int? layerId, string? searchString, int pageNumber, int pageSize)
        {
            var data = _unitOfWork.LayerItemRepository.EnableQuery();
            var result = DataQuery(data, layerId, layerItemId, searchString, pageNumber, pageSize);

            return result;
        }

        public LayerItem Update(int layerItemId, LayerItemUpdateDTO layerItemUpdateDTO)
        {
            var data = _unitOfWork.LayerItemRepository.Find(c => c.LayerItemId == layerItemId && c.IsDeleted == false).FirstOrDefault()
            ?? throw new Exception("Layer item not found or deleted");

            _mapper.Map(layerItemUpdateDTO, data);
            _unitOfWork.LayerItemRepository.Update(data);
            _unitOfWork.Save();

            UpdateDisplayIfExist(data);

            return data;
        }


        private IEnumerable<LayerItem> DataQuery(IQueryable<LayerItem> data, int? layerId, int? layerItemId, string? searchString, int pageNumber, int pageSize)
        {
            data = data.Where(data => data.IsDeleted == false);

            if (layerItemId != null)
            {
                data = data
                    .Where(c => c.LayerItemId == layerItemId);
            }

            if (layerId != null)
            {
                data = data
                    .Where(c => c.LayerId == layerId);
            }

            if (searchString != null)
            {
                searchString = searchString.Trim();
                data = data
                    .Where(c => c.LayerItemValue.Contains(searchString));
            }
            return PaginatedList<LayerItem>.Create(data, pageNumber, pageSize);
        }

        private void UpdateDisplayIfExist(LayerItem data)
        {
            // Find the display associated with the template and check if it exists and is not deleted
            var display = _unitOfWork.DisplayRepository.EnableQuery()
                .Include(c => c.Template!)
                    .ThenInclude(c => c.Layers!.Where(d => d.LayerId == data.LayerId && !d.IsDeleted))
                .Where(c => !c.Template!.IsDeleted)
                .FirstOrDefault();

            // If the display exists, mark it as changed and save the changes
            if (display != null)
            {
                display.IsChanged = true;
                _unitOfWork.DisplayRepository.Update(display);
                _unitOfWork.Save();
            }
        }
    }
}