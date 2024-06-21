using SmartMenu.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartMenu.Domain.Repository
{
    public interface IDisplayItemRepository : IGenericRepository<DisplayItem>
    {
        IEnumerable<DisplayItem> GetAll(int displayItemId, int displayId, int boxId, int productGroupId, string? searchString, int pageNumber, int pageSize);
    }
}
