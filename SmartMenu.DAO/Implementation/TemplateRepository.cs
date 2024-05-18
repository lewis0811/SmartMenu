using Microsoft.EntityFrameworkCore;
using SmartMenu.Domain.Models;
using SmartMenu.Domain.Repository;

namespace SmartMenu.DAO.Implementation
{
    public class TemplateRepository : GenericRepository<Template>, ITemplateRepository
    {
        private readonly SmartMenuDBContext _context;

        public TemplateRepository(SmartMenuDBContext context) : base(context)
        {
            _context = context;
        }

        public IEnumerable<Template> GetAll(int? templateId, string? searchString, int pageNumber, int pageSize)
        {
            var data = _context.Templates.AsQueryable();
            return DataQuery(data, templateId, searchString, pageNumber, pageSize);
        }

        public IEnumerable<Template> GetAllWithLayers(int? templateId, string? searchString, int pageNumber = 1, int pageSize = 10)
        {
            var data = _context.Templates
                .Include(c => c.Layers)
                .AsQueryable();
            return DataQuery(data, templateId, searchString, pageNumber, pageSize);
        }

        private IEnumerable<Template> DataQuery(IQueryable<Template> data, int? templateId, string? searchString, int pageNumber, int pageSize)
        {
            data = data.Where(c => c.IsDeleted == false);
            if (templateId != null)
            {
                data = data
                    .Where(c => c.TemplateID == templateId);
            }

            if (searchString != null)
            {
                searchString = searchString.Trim();
                data = data
                    .Where(c => c.TemplateName.Contains(searchString)
                    || c.TemplateDescription.Contains(searchString));
            }

            return PaginatedList<Template>.Create(data, pageNumber, pageSize);
        }
    }
}