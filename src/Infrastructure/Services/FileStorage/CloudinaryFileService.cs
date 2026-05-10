using Application.Configurations;
using Application.Services.Contracts;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;


namespace Infrastructure.Services.FileStorage
{
    public class CloudinaryFileService : IFileService
    {

        private readonly Cloudinary _cloudClient;
        private readonly ILogger<CloudinaryFileService> _logger;
        private readonly CloudinarySettings _cloudinaryConfig;


        
        
        public CloudinaryFileService(IOptions<CloudinarySettings> config, ILogger<CloudinaryFileService> logger)
        {
            _logger = logger;
            _cloudinaryConfig = config.Value;
            // Only initialize if credentials are provided
            if (!string.IsNullOrWhiteSpace(_cloudinaryConfig.CloudName) &&
                !string.IsNullOrWhiteSpace(_cloudinaryConfig.ApiKey) &&
                !string.IsNullOrWhiteSpace(_cloudinaryConfig.ApiSecret))
            {
                var account = $"cloudinary://{_cloudinaryConfig.ApiKey}:{_cloudinaryConfig.ApiSecret}@{_cloudinaryConfig.CloudName}";
                _cloudClient = new Cloudinary(account);
            }
        }

        public async Task<bool> DeleteFile(string publicId)
        {
            var deleteParams = new DeletionParams(publicId);
            var result = await _cloudClient.DestroyAsync(deleteParams);
            return result.Result == "ok";
        }

        [Obsolete]
        public async Task<string> UploadFile(IFormFile file, string fileName)
        {

            if (_cloudClient == null)
                throw new InvalidOperationException("Cloudinary is not configured. Check your CloudinarySettings.");
            await using var stream = file.OpenReadStream();

            var uploadParams = new ImageUploadParams
            {
                File = new FileDescription(fileName, stream)
            };

            var uploadResult = await _cloudClient.UploadAsync(uploadParams);
            return uploadResult.SecureUri.ToString();
        }


    }
}