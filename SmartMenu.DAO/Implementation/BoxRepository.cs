﻿using Microsoft.EntityFrameworkCore;
using SmartMenu.Domain.Models;
using SmartMenu.Domain.Repository;

namespace SmartMenu.DAO.Implementation
{
    public class BoxRepository : GenericRepository<Box>, IBoxRepository
    {
        private readonly SmartMenuDBContext _context;

        public BoxRepository(SmartMenuDBContext context) : base(context)
        {
            _context = context;
        }

        public IEnumerable<Box> GetAll(int? boxId, int? layerId, string? searchString, int pageNumber, int pageSize)
        {
            var data = _context.Boxes.AsQueryable();
            return DataQuery(data, boxId, layerId, searchString, pageNumber, pageSize);
        }

        public IEnumerable<Box> GetAllWithBoxItems(int? boxId, int? layerId, string? searchString, int pageNumber, int pageSize)
        {
            var data = _context.Boxes
                .Include(c => c.BoxItems)!
                .ThenInclude(c => c.BFont)
                .AsQueryable();
            return DataQuery(data, boxId, layerId, searchString, pageNumber, pageSize);
        }

        private IEnumerable<Box> DataQuery(IQueryable<Box> data, int? boxId, int? layerId, string? searchString, int pageNumber, int pageSize)
        {
            data = data.Where(data => data.IsDeleted == false);

            if (boxId != null)
            {
                data = data
                    .Where(c => c.BoxId == boxId);
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
                    .Where(c =>
                     c.BoxPositionX.ToString().Contains(searchString)
                    || c.BoxPositionY.ToString().Contains(searchString));
            }

            return PaginatedList<Box>.Create(data, pageNumber, pageSize);
        }
    }
}