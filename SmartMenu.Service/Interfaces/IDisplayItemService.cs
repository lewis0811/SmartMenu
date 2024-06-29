using SmartMenu.Domain.Models;
using SmartMenu.Domain.Models.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartMenu.Service.Interfaces
{
    public interface IDisplayItemService
    {
        DisplayItem AddDisplayItem(DisplayItemCreateDTO displayItemCreateDTO);
        void Delete(int displayItemId);
        IEnumerable<DisplayItem> GetAll(int? displayItemId, int? displayId, int? boxId, int? productGroupId, string? searchString, int pageNumber, int pageSize);
        DisplayItem Update(int displayItemId, DisplayItemUpdateDTO displayItemUpdateDTO);
    }
}
