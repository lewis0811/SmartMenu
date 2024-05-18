using SmartMenu.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartMenu.Domain.Repository
{
    public interface ITemplateRepository : IGenericRepository<Template>
    {
        IEnumerable<Template> GetAll(int? templateId, string? searchString, int pageNumber, int pageSize);
        IEnumerable<Template> GetAllWithLayers(int? templateId, string? searchString, int pageNumber = 1, int pageSize = 10);
    }
}
