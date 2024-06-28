using Microsoft.EntityFrameworkCore;
using SmartMenu.Domain.Models;
using SmartMenu.Domain.Repository;

namespace SmartMenu.DAO.Implementation
{
    public class CollectionRepository : GenericRepository<Collection>, ICollectionRepository
    {
        private readonly SmartMenuDBContext _context;

        public CollectionRepository(SmartMenuDBContext context) : base(context)
        {
            _context = context;
        }

        public IEnumerable<Collection> GetAll(int? collectionId, int? brandId, string? searchString, int pageNumber, int pageSize)
        {
            var data = _context.Collections.AsQueryable();

            return DataQuery(data, collectionId, brandId, searchString, pageNumber, pageSize);
        }
        public IEnumerable<Collection> GetCollectionWithProductGroup(int? collectionId, int? brandId, string? searchString, int pageNumber, int pageSize)
        {
            var data = _context.Collections.Include(x => x.ProductGroups).AsQueryable();

            return DataQuery(data, collectionId, brandId, searchString, pageNumber, pageSize);
        }

        private IEnumerable<Collection> DataQuery(IQueryable<Collection> data, int? collectionId, int? brandId, string? searchString, int pageNumber, int pageSize)
        {
            data = data.Where(c => c.IsDeleted == false);
            if (collectionId != null)
            {
                data = data
                    .Where(c => c.CollectionId == collectionId);
            }

            if (brandId != null)
            {
                data = data
                    .Where(c => c.BrandId == brandId);
            }

            if (searchString != null)
            {
                searchString = searchString.Trim();
                data = data
                    .Where(c => c.CollectionName.Contains(searchString)
                    || c.CollectionDescription!.Contains(searchString));
            }

            return PaginatedList<Collection>.Create(data, pageNumber, pageSize);
        }
    }
}
