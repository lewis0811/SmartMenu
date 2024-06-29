using SmartMenu.Domain.Models;
using SmartMenu.Domain.Models.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartMenu.Service.Interfaces
{
    public interface IBoxItemService
    {
        BoxItem AddBoxItem(BoxItemCreateDTO boxItemCreateDTO);
        void Delete(int boxItemId);
        IEnumerable<BoxItem> GetAll(int? boxItemId, int? boxId, int? fontId, string? searchString, int pageNumber, int pageSize);
        BoxItem Update(int boxItemId, BoxItemUpdateDTO boxItemUpdateDTO);
    }
}
