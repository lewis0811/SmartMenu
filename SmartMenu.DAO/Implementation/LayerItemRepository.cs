﻿using SmartMenu.Domain.Models;
using SmartMenu.Domain.Repository;

namespace SmartMenu.DAO.Implementation
{
    public class LayerItemRepository : GenericRepository<LayerItem>, ILayerItemRepository
    {
        private readonly SmartMenuDBContext _context;

        public LayerItemRepository(SmartMenuDBContext context) : base(context)
        {
            _context = context;
        }

        public IEnumerable<LayerItem> GetAll(int? layerItemId, int? layerId, string? searchString, int pageNumber, int pageSize)
        {
            var data = _context.LayersItem.AsQueryable();
            return DataQuery(data, layerItemId, layerId, searchString, pageNumber, pageSize);
        }

        private IEnumerable<LayerItem> DataQuery(IQueryable<LayerItem> data, int? layerItemId, int? layerId, string? searchString, int pageNumber, int pageSize)
        {
            data = data.Where(data => data.IsDeleted == false);

            if (layerItemId != null)
            {
                data = data
                    .Where(c => c.LayerItemID == layerItemId);
            }

            if (layerId != null)
            {
                data = data
                    .Where(c => c.LayerID == layerId);
            }

            if (searchString != null)
            {
                searchString = searchString.Trim();
                data = data
                    .Where(c => c.LayerItemValue.Contains(searchString)
                    || c.LayerItemType.ToString().Contains(searchString));
            }
            return PaginatedList<LayerItem>.Create(data, pageNumber, pageSize);
        }
    }
}