using Microsoft.AspNetCore.Http;

namespace SmartMenu.Domain.Repository
{
    public interface IBlobRepository
    {
        string GetBlob(string blobName, string containerName);
        Task<bool> DeleteBlob(string blobName, string containerName);
        Task<string> UploadBlob(string blobName, string containerName, IFormFile formFile);
    }
}
