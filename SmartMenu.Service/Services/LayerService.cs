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
                .Include(c => c.LayerItem).Where(c => !c.LayerItem!.IsDeleted)
                .Include(c => c.Boxes!.Where(d => !d.IsDeleted))
                    .ThenInclude(c => c.BoxItems);

            var result = DataQuery(data, layerId, templateId, searchString, pageNumber, pageSize);

            return result ?? Enumerable.Empty<Domain.Models.Layer>();
        }

        public IEnumerable<Domain.Models.Layer> GetAllWithLayerItems(int? layerId, int? templateId, string? searchString, int pageNumber, int pageSize)
        {
            var data = _unitOfWork.LayerRepository.EnableQuery();
            data = data.Include(c => c.LayerItem).Where(c => !c.IsDeleted);

            var result = DataQuery(data, layerId, templateId, searchString, pageNumber, pageSize);

            return result ?? Enumerable.Empty<Domain.Models.Layer>();
        }

        public IEnumerable<Domain.Models.Layer> GetAllWithBoxes(int? layerId, int? templateId, string? searchString, int pageNumber, int pageSize)
        {
            var data = _unitOfWork.LayerRepository.EnableQuery();
            data = data.Include(c => c.Boxes!.Where(d => !d.IsDeleted));

            var result = DataQuery(data, layerId, templateId, searchString, pageNumber, pageSize);

            return result ?? Enumerable.Empty<Domain.Models.Layer>();
        }


        /// <summary>
        /// Adds a new layer to the system.
        /// </summary>
        /// <param name="layerCreateDTO">The DTO containing the layer information.</param>
        /// <returns>The newly created layer.</returns>
        /// <exception cref="Exception">Thrown when the template is not found or deleted.</exception>
        /// <exception cref="Exception">Thrown when a background image layer already exists.</exception>
        public async Task<Domain.Models.Layer> AddLayerAsync(LayerCreateDTO layerCreateDTO)
        {
            // Find the template by its ID and check if it exists and is not deleted
            var template = _unitOfWork.TemplateRepository.Find(c => c.TemplateId == layerCreateDTO.TemplateId && c.IsDeleted == false).FirstOrDefault()
                ?? throw new Exception("Template not found or deleted");

            // Map the DTO to a new layer object
            var data = _mapper.Map<Domain.Models.Layer>(layerCreateDTO);

            // If the layer type is background image, check if there is already a background image layer
            if (data.LayerType == LayerType.BackgroundImage)
            {
                var isExistLayer = _unitOfWork.LayerRepository.EnableQuery().Any(x => x.TemplateId == data.TemplateId && x.LayerType == LayerType.BackgroundImage);
                if (isExistLayer) throw new Exception("Background image layer already exists");

                // Set the layer type to 0 and z-index to 0
                data.LayerType = 0;
                data.ZIndex = 0;
            }
            else
            {
                // Check if there are any layers for the template
                var isExistLayer = _unitOfWork.LayerRepository.EnableQuery().Any(x => x.TemplateId == data.TemplateId);

                switch (isExistLayer)
                {
                    case false:
                        // If there are no layers, set the z-index to 1
                        data.ZIndex = 1;
                        break;

                    case true:
                        // If there are layers, get the highest z-index and increment it by 1
                        var recentZIndex = await _unitOfWork.LayerRepository.EnableQuery()
                            .Where(c => c.TemplateId == data.TemplateId)
                            .Select(c => c.ZIndex).MaxAsync();
                        data.ZIndex = recentZIndex + 1;
                        break;
                }
            }

            // Add the new layer to the repository and save the changes
            _unitOfWork.LayerRepository.Add(data);
            _unitOfWork.Save();

            UpdateDisplayIfExist(data);

            // Return the newly created layer
            return data;
        }

        public Domain.Models.Layer Update(int layerId, LayerUpdateDTO layerUpdateDTO)
        {
            var data = _unitOfWork.LayerRepository.Find(c => c.LayerId == layerId && c.IsDeleted == false).FirstOrDefault()
            ?? throw new Exception("Template not found or deleted");

            _mapper.Map(layerUpdateDTO, data);
            _unitOfWork.LayerRepository.Update(data);
            _unitOfWork.Save();

            UpdateDisplayIfExist(data);

            return data;
        }

        public void Delete(int layerId)
        {
            var data = _unitOfWork.LayerRepository.Find(c => c.LayerId == layerId && c.IsDeleted == false).FirstOrDefault()
            ?? throw new Exception("Template not found or deleted");

            //data.IsDeleted = true;

            _unitOfWork.LayerRepository.Remove(data);
            _unitOfWork.Save();

            UpdateDisplayIfExist(data);
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

        private void UpdateDisplayIfExist(Layer data)
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