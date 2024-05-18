using SmartMenu.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartMenu.Domain.Repository
{
    public interface IBoxRepository : IGenericRepository<Box>
    {
        IEnumerable<Box> GetAll(int? boxId, int? layerId, int? fontId, string? searchString, int pageNumber, int pageSize);
    }
}
