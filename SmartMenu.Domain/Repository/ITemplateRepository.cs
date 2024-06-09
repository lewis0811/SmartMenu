using SmartMenu.Domain.Models;

namespace SmartMenu.Domain.Repository
{
    public interface ITemplateRepository : IGenericRepository<Template>
    {
        IEnumerable<Template> GetAll(int? templateId, string? searchString, int pageNumber, int pageSize);
        IEnumerable<Template> GetAllWithLayers(int? templateId, string? searchString, int pageNumber = 1, int pageSize = 10);
    }
}
