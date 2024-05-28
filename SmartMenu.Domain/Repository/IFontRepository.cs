using SmartMenu.Domain.Models;
using SmartMenu.Domain.Models.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartMenu.Domain.Repository
{
    public interface IFontRepository : IGenericRepository<Font>
    {
        IEnumerable<Font> GetAll(int? fontId, string searchString, int pageNumber, int pageSize);
        void Add(FontCreateDTO font);
    }
}
