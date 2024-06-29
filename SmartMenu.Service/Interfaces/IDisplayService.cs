using SmartMenu.Domain.Models;
using SmartMenu.Domain.Models.DTO;

namespace SmartMenu.Service.Interfaces
{
    public interface IDisplayService
    {
        IEnumerable<Display> GetAll(int? displayId, int? menuId, int? collectionId, string? searchString, int pageNumber, int pageSize);

        IEnumerable<Display> GetWithItems(int? displayId, int? menuId, int? collectionId, string? searchString, int pageNumber, int pageSize);

        Display AddDisplay(DisplayCreateDTO displayCreateDTO);

        Display AddDisplayV2(DisplayCreateDTO displayCreateDTO);

        Display Update(int displayId, DisplayUpdateDTO displayUpdateDTO);

        void Delete(int displayId);
    }
}