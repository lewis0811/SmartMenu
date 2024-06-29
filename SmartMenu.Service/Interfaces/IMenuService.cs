using SmartMenu.Domain.Models;
using SmartMenu.Domain.Models.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartMenu.Service.Interfaces
{
    public interface IMenuService
    {
        IEnumerable<Menu> GetAll(int? menuId, int? brandId, string? searchString, int pageNumber, int pageSize);
        IEnumerable<Menu> GetMenuWithProductGroup(int? menuId, int? brandId, string? searchString, int pageNumber, int pageSize);
        Menu Add(MenuCreateDTO menuCreateDTO);
        Menu Update(int menuId, MenuUpdateDTO menuUpdateDTO);
        void Delete(int menuId);
    }
}
