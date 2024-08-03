using SmartMenu.Domain.Models;
using SmartMenu.Domain.Models.DTO;

namespace SmartMenu.Domain.Repository
{
    public interface IFontRepository : IGenericRepository<BFont>
    {
        IEnumerable<BFont> GetAll(int? fontId, string? searchString, int pageNumber, int pageSize);
        void Add(FontCreateDTO data, string path);
    }
}
