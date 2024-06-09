using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.AspNetCore.Http;
using SmartMenu.Domain.Repository;

namespace SmartMenu.DAO.Implementation
{
    public class BlobRepository : IBlobRepository
    {
        private readonly BlobServiceClient _blobClient;

        public BlobRepository(BlobServiceClient blobClient)
        {
            _blobClient = blobClient;
        }

        public BlobClient GetBlobClient(string blobName, string containerName)
        {
            BlobContainerClient blobContainerClient = _blobClient.GetBlobContainerClient(containerName);
            BlobClient blobClient = blobContainerClient.GetBlobClient(blobName);

            return blobClient;
        }

        public async Task<bool> DeleteBlob(string blobName, string containerName)
        {
            return await GetBlobClient(blobName, containerName).DeleteIfExistsAsync();
        }

        public string GetBlob(string blobName, string containerName)
        {
            return GetBlobClient(blobName, containerName).Uri.AbsoluteUri;
        }

        public async Task<string> UploadBlob(string blobName, string containerName, IFormFile formFile)
        {
            var blobClient = GetBlobClient(blobName, containerName);
            var httpHeaders = new BlobHttpHeaders()
            {
                ContentType = formFile.ContentType
            };

            var result = await blobClient.UploadAsync(formFile.OpenReadStream(), httpHeaders);

            return result != null
                ? GetBlob(blobName, containerName)
                : throw new Exception("Upload fail");

        }
    }
}
