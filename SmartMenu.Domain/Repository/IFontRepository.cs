using SmartMenu.Domain.Models;
using SmartMenu.Domain.Models.DTO;

namespace SmartMenu.Domain.Repository
{
    public interface IFontRepository : IGenericRepository<Font>
    {
        IEnumerable<Font> GetAll(int? fontId, string? searchString, int pageNumber, int pageSize);
        void Add(FontCreateDTO data, string path);
    }
}
