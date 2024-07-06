using AutoMapper;
using SmartMenu.DAO;
using SmartMenu.Domain.Models;
using SmartMenu.Domain.Models.DTO;
using SmartMenu.Domain.Repository;
using SmartMenu.Service.Interfaces;

namespace SmartMenu.Service.Services
{
    public class LayerItemService : IlayerItemService
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

            var data = _mapper.Map<LayerItem>(layerItemCreateDTO);

            _unitOfWork.LayerItemRepository.Add(data);
            _unitOfWork.Save();

            return data;
        }

        public void Delete(int layerItemId)
        {
            var data = _unitOfWork.LayerItemRepository.Find(c => c.LayerItemId == layerItemId && c.IsDeleted == false).FirstOrDefault()
            ?? throw new Exception("Layer item not found or deleted");

            data.IsDeleted = true;

            _unitOfWork.LayerItemRepository.Update(data);
            _unitOfWork.Save();
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
    }
}