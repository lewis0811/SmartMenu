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
        Display AddDisplayV3(DisplayCreateDTO displayCreateDTO);
        string GetImageById(int displayId);
        string UpdateContainImage(int displayId, DisplayUpdateDTO displayUpdateDTO);
        Display AddDisplayV4(DisplayCreateDTO displayCreateDTO, string tempPath);
        string GetImageByIdV2(int displayId, string tempPath);
    }
}