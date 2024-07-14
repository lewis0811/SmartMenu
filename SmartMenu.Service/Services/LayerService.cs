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
    public class LayerService : ILayerService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public LayerService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public IEnumerable<Layer> GetAll(int? layerId, int? templateId, string? searchString, int pageNumber, int pageSize)
        {
            var data = _unitOfWork.LayerRepository.EnableQuery();
            var result = DataQuery(data, layerId, templateId, searchString, pageNumber, pageSize);

            return result ?? Enumerable.Empty<Layer>();
        }

        public IEnumerable<Layer> GetAllWithLayerItemsAndBoxes(int? layerId, int? templateId, string? searchString, int pageNumber, int pageSize)
        {
            var data = _unitOfWork.LayerRepository.EnableQuery();
            data = data
                .Include(c => c.LayerItem)
                .Include(c => c.Boxes)!
                .ThenInclude(c => c.BoxItems);

            var result = DataQuery(data, layerId, templateId, searchString, pageNumber, pageSize);

            return result ?? Enumerable.Empty<Layer>();
        }

        public IEnumerable<Layer> GetAllWithLayerItems(int? layerId, int? templateId, string? searchString, int pageNumber, int pageSize)
        {
            var data = _unitOfWork.LayerRepository.EnableQuery();
            data = data.Include(c => c.LayerItem);

            var result = DataQuery(data, layerId, templateId, searchString, pageNumber, pageSize);

            return result ?? Enumerable.Empty<Layer>();
        }

        public IEnumerable<Layer> GetAllWithBoxes(int? layerId, int? templateId, string? searchString, int pageNumber, int pageSize)
        {
            var data = _unitOfWork.LayerRepository.EnableQuery();
            data = data.Include(c => c.Boxes);

            var result = DataQuery(data, layerId, templateId, searchString, pageNumber, pageSize);

            return result ?? Enumerable.Empty<Layer>();
        }

        private IEnumerable<Layer> DataQuery(IQueryable<Layer> data, int? layerId, int? templateId, string? searchString, int pageNumber, int pageSize)
        {
            data = data.Where(data => data.IsDeleted == false);

            if (templateId != null)
            {
                data = data.Where(c => c.TemplateId == templateId);
            }

            if (layerId != null)
            {
                data = data.Where(c => c.LayerId == layerId);
            }

            if (searchString != null)
            {
                searchString = searchString.Trim();
                data = data
                    .Where(c => c.LayerName.Contains(searchString));
            }
            return PaginatedList<Layer>.Create(data, pageNumber, pageSize);
        }

        public Layer AddLayer(LayerCreateDTO layerCreateDTO)
        {
            var template = _unitOfWork.TemplateRepository.Find(c => c.TemplateId == layerCreateDTO.TemplateID && c.IsDeleted == false).FirstOrDefault()
            ?? throw new Exception("Template not found or deleted");

            if (layerCreateDTO.LayerType == LayerType.BackgroundImageLayer)
            {
                var existedLayer = _unitOfWork.LayerRepository
                    .Find(c => c.TemplateId == template.TemplateId && c.LayerType == LayerType.BackgroundImageLayer && c.IsDeleted == false)
                    .FirstOrDefault();
                if (existedLayer != null) throw new Exception($"BackgroundImageLayer already exist in template ID: {template.TemplateId}");
            }

            if (layerCreateDTO.LayerType == LayerType.MenuCollectionNameLayer) 
            {
                var existedLayer = _unitOfWork.LayerRepository
              .Find(c => c.TemplateId == template.TemplateId && c.LayerType == LayerType.MenuCollectionNameLayer && c.IsDeleted == false)
              .FirstOrDefault();
                if (existedLayer != null) throw new Exception($"MenuCollectionNameLayer already exist in template ID: {template.TemplateId}");
            }

            var data = _mapper.Map<Layer>(layerCreateDTO);

            _unitOfWork.LayerRepository.Add(data);
            _unitOfWork.Save();

            return data;
        }

        public Layer Update(int layerId, LayerUpdateDTO layerUpdateDTO)
        {
            var data = _unitOfWork.LayerRepository.Find(c => c.LayerId == layerId && c.IsDeleted == false).FirstOrDefault()
            ?? throw new Exception("Template not found or deleted");

            _mapper.Map(layerUpdateDTO, data);
            _unitOfWork.LayerRepository.Update(data);
            _unitOfWork.Save();

            return data;
        }

        public void Delete(int layerId)
        {
            var data = _unitOfWork.LayerRepository.Find(c => c.LayerId == layerId && c.IsDeleted == false).FirstOrDefault()
            ?? throw new Exception ("Template not found or deleted");

            data.IsDeleted = true;

            _unitOfWork.LayerRepository.Update(data);
            _unitOfWork.Save();
        }
    }
}