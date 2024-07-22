using SmartMenu.Domain.Models;
using SmartMenu.Domain.Models.DTO;

namespace SmartMenu.Service.Interfaces
{
    public interface IDisplayService
    {
        IEnumerable<Display> GetAll(int? displayId, int? menuId, int? collectionId, string? searchString, int pageNumber, int pageSize);

        IEnumerable<Display> GetWithItems(int? displayId, int? menuId, int? collectionId, string? searchString, int pageNumber, int pageSize);

        Task<string> GetImageByTimeAsync(int deviceId, string tempPath);

        Task<string> GetImageByIdAsync(Display display, string tempPath);

        Task<string> GetImageByIdV2Async(int displayId, string tempPath);

        Display AddDisplay(DisplayCreateDTO displayCreateDTO);

        Task<Display> AddDisplayV4Async(DisplayCreateDTO displayCreateDTO, string tempPath);

        //Display AddDisplayV2(DisplayCreateDTO displayCreateDTO);
        Task<string> UpdateContainImageAsync(int displayId, DisplayUpdateDTO displayUpdateDTO, string tempPath);

        Display Update(int displayId, DisplayUpdateDTO displayUpdateDTO);

        void Delete(int displayId);

        void DeleteTempFile(string tempPath);

        //Display AddDisplayV3(DisplayCreateDTO displayCreateDTO);
    }
}