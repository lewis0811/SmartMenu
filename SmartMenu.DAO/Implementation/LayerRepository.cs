﻿using Microsoft.EntityFrameworkCore;
using SmartMenu.Domain.Models;
using SmartMenu.Domain.Repository;

namespace SmartMenu.DAO.Implementation
{
    public class LayerRepository : GenericRepository<Layer>, ILayerRepository
    {
        private readonly SmartMenuDBContext _context;

        public LayerRepository(SmartMenuDBContext context) : base(context)
        {
            _context = context;
        }

        public IEnumerable<Layer> GetAll(int? layerId, int? templateId, string? searchString, int pageNumber, int pageSize)
        {
            var data = _context.Layers.AsQueryable();
            return DataQuery(data, layerId, templateId, searchString, pageNumber, pageSize);
        }

        public IEnumerable<Layer> GetAllWithBoxes(int? layerId, int? templateId, string? searchString, int pageNumber, int pageSize)
        {
            var data = _context.Layers
                            .Include(c => c.Boxes)
                            .AsQueryable();
            return DataQuery(data, layerId, templateId, searchString, pageNumber, pageSize);
        }

        public IEnumerable<Layer> GetAllWithLayerItems(int? layerId, int? templateId, string? searchString, int pageNumber, int pageSize)
        {
            var data = _context.Layers
                .Include(c => c.LayerItem)
                .AsQueryable();
            return DataQuery(data, layerId, templateId, searchString, pageNumber, pageSize);
        }

        public IEnumerable<Layer> GetAllWithLayerItemsAndBoxes(int? layerId, int? templateId, string? searchString, int pageNumber, int pageSize)
        {
            var data = _context.Layers
                .Include(c => c.LayerItem)
                .Include(c => c.Boxes)!
                .ThenInclude(c => c.BoxItems)
                .AsQueryable();
            return DataQuery(data, layerId, templateId, searchString, pageNumber, pageSize);
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
                    .Where(c => c.LayerType.ToString().Contains(searchString));
            }
            return PaginatedList<Layer>.Create(data, pageNumber, pageSize);
        }
    }
}