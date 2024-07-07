using SmartMenu.Domain.Models;
using SmartMenu.Domain.Models.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartMenu.Service.Interfaces
{
    public interface IFontService
    {
        void Add(FontCreateDTO fontCreateDTO, string path);
        IEnumerable<Font> GetAll(int? fontId, string? searchString, int pageNumber, int pageSize);
    }
}
