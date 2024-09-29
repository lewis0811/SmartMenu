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
        Task AddAsync(FontCreateDTO fontCreateDTO, string path);
        void Delete(int fontId);
        IEnumerable<BFont> GetAll(int? fontId, string? searchString, int pageNumber, int pageSize);
    }
}
