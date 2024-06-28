using Amazon.S3;
using Microsoft.AspNetCore.Mvc;
using SmartMenu.Domain.Repository;
using System.Security.AccessControl;

namespace SmartMenu.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BucketsController : ControllerBase
    {
        private readonly IAmazonS3 _s3Client;
        private readonly string BucketName;
        private readonly IUnitOfWork _unitOfWork;

        public BucketsController(IAmazonS3 s3Client, IConfiguration configuration, IUnitOfWork unitOfWork)
        {
            _s3Client = s3Client;
            BucketName = configuration.GetValue<string>("AWS:BucketName");
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllBucketsAsync()
        {
            var data = await _s3Client.ListBucketsAsync();

            var buckets = data.Buckets.Select(c => c.BucketName).ToList();

            return Ok(buckets);
        }

        [HttpGet("Objects")]
        public async Task<IActionResult> GetAllObjectsInBucketAsync(int? templateId, int? displayId)
        {
            var bucketExist = await Amazon.S3.Util.AmazonS3Util.DoesS3BucketExistV2Async(_s3Client, BucketName);
            if (!bucketExist) return BadRequest($"Bucket {BucketName} not found");

            var data = await _s3Client.ListObjectsAsync(BucketName);
            var responseData = data.S3Objects;

            if (templateId != null)
            {
                var template = _unitOfWork.TemplateRepository.Find(c => c.TemplateId == templateId && c.IsDeleted == false).FirstOrDefault();
                if (template == null) return BadRequest("Template not found or deleted");

                responseData = responseData.Where(c => c.Key.Contains(template.TemplateName.Trim())).ToList();
            }

            if (displayId != null)
            {
                var display = _unitOfWork.TemplateRepository.Find(c => c.TemplateId == displayId && c.IsDeleted == false).FirstOrDefault();
                if (display == null) return BadRequest("Display not found or deleted");

                
            }

            return Ok(responseData);
        }
    }
}