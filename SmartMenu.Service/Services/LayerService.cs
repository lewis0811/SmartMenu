using AutoMapper;
using CloudinaryDotNet;
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

        public IEnumerable<Domain.Models.Layer> GetAll(int? layerId, int? templateId, string? searchString, int pageNumber, int pageSize)
        {
            var data = _unitOfWork.LayerRepository.EnableQuery();
            var result = DataQuery(data, layerId, templateId, searchString, pageNumber, pageSize);

            return result ?? Enumerable.Empty<Domain.Models.Layer>();
        }

        public IEnumerable<Domain.Models.Layer> GetAllWithLayerItemsAndBoxes(int? layerId, int? templateId, string? searchString, int pageNumber, int pageSize)
        {
            var data = _unitOfWork.LayerRepository.EnableQuery();
            data = data
                .Include(c => c.LayerItem)
                .Include(c => c.Boxes)!
                .ThenInclude(c => c.BoxItems);

            var result = DataQuery(data, layerId, templateId, searchString, pageNumber, pageSize);

            return result ?? Enumerable.Empty<Domain.Models.Layer>();
        }

        public IEnumerable<Domain.Models.Layer> GetAllWithLayerItems(int? layerId, int? templateId, string? searchString, int pageNumber, int pageSize)
        {
            var data = _unitOfWork.LayerRepository.EnableQuery();
            data = data.Include(c => c.LayerItem);

            var result = DataQuery(data, layerId, templateId, searchString, pageNumber, pageSize);

            return result ?? Enumerable.Empty<Domain.Models.Layer>();
        }

        public IEnumerable<Domain.Models.Layer> GetAllWithBoxes(int? layerId, int? templateId, string? searchString, int pageNumber, int pageSize)
        {
            var data = _unitOfWork.LayerRepository.EnableQuery();
            data = data.Include(c => c.Boxes);

            var result = DataQuery(data, layerId, templateId, searchString, pageNumber, pageSize);

            return result ?? Enumerable.Empty<Domain.Models.Layer>();
        }

        private IEnumerable<Domain.Models.Layer> DataQuery(IQueryable<Domain.Models.Layer> data, int? layerId, int? templateId, string? searchString, int pageNumber, int pageSize)
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
                    .Where(c => c.LayerType.ToString().Contains(searchString));
            }
            return PaginatedList<Domain.Models.Layer>.Create(data, pageNumber, pageSize);
        }

        public async Task<Domain.Models.Layer> AddLayerAsync(LayerCreateDTO layerCreateDTO)
        {
            var template = _unitOfWork.TemplateRepository.Find(c => c.TemplateId == layerCreateDTO.TemplateId && c.IsDeleted == false).FirstOrDefault()
            ?? throw new Exception("Template not found or deleted");

            var data = _mapper.Map<Domain.Models.Layer>(layerCreateDTO);

            if (data.LayerType == LayerType.BackgroundImage)
            {
                var isExistLayer = _unitOfWork.LayerRepository.EnableQuery().Any(x => x.TemplateId == data.TemplateId && x.LayerType == LayerType.BackgroundImage);
                if (isExistLayer) throw new Exception("Background image layer already exists");

                data.LayerType = 0;
                data.ZIndex = 0;
            }
            else
            {
                var isExistLayer = _unitOfWork.LayerRepository.EnableQuery().Any(x => x.TemplateId == data.TemplateId);

                switch (isExistLayer)
                {
                    case false:
                        data.ZIndex = 1;
                        break;
                    case true:
                        var recentZIndex = await _unitOfWork.LayerRepository.EnableQuery()
                            .Where(c => c.TemplateId == data.TemplateId)
                            .Select(c => c.ZIndex).MaxAsync();
                        data.ZIndex = recentZIndex + 1;
                        break;
                }
            }

            _unitOfWork.LayerRepository.Add(data);
            _unitOfWork.Save();

            return data;
        }

        public Domain.Models.Layer Update(int layerId, LayerUpdateDTO layerUpdateDTO)
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