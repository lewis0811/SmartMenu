using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Http;
using SmartMenu.Domain.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartMenu.DAO.Implementation
{
    public class BlobRepository : IBlobRepository
    {
        private readonly BlobServiceClient _blobClient;

        public BlobRepository(BlobServiceClient blobClient)
        {
            _blobClient = blobClient;
        }

        public Task<bool> DeleteBlob(string blobName, string containerName)
        {
            throw new NotImplementedException();
        }

        public string GetBlob(string blobName, string containerName)
        {
            throw new NotImplementedException();
        }

        public Task<string> UploadBlob(string blobName, string containerName, IFormFile formFile)
        {
            throw new NotImplementedException();
        }
    }
}
