using SmartMenu.Domain.Models;
using SmartMenu.Domain.Models.DTO;

namespace SmartMenu.Service.Interfaces
{
    public interface IDisplayService
    {
        IEnumerable<Display> GetAll(int? displayId, int? menuId, int? collectionId, string? searchString, int pageNumber, int pageSize);

        IEnumerable<Display> GetWithItems(int? displayId, int? menuId, int? collectionId, string? searchString, int pageNumber, int pageSize);

        Task<string> GetImageByTimeAsync(int deviceId);

        Task<string> GetImageByDisplayAsync(int displayId);
        Task<string> GetTemplateImageAsync(int displayId);

        Display AddDisplay(DisplayCreateDTO displayCreateDTO);

        Task<Display> AddDisplayV2Async(DisplayCreateDTO displayCreateDTO);

        Display Update(int displayId, DisplayUpdateDTO displayUpdateDTO);

        void Delete(int displayId);

        void DeleteTempFile();
        Task<Display> GetByDeviceId(int? deviceId);
        IEnumerable<Display> GetWithItemsV2(int storeId, int? deviceId, int? displayId, int? menuId, int? collectionId, string? searchString, int pageNumber, int pageSize);
    }
}