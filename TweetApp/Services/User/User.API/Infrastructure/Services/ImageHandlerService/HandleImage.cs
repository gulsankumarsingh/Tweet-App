using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using User.API.Infrastructure.Services.ImageHandlerService.Interface;
using User.API.Models;

namespace User.API.Infrastructure.Services.ImageHandlerService
{
    public class HandleImage : IHandleImage
    {
        private readonly string _storeConnectionString;
        private readonly string _containerName;
        private readonly ILogger<HandleImage> _logger;

        public HandleImage(IConfiguration configuration, ILogger<HandleImage> logger)
        {
            _storeConnectionString = configuration.GetValue<string>("AzureStorageAccount:ConnectionStrings");
            _containerName = configuration.GetValue<string>("AzureStorageAccount:ContainerName");
            _logger = logger;
        }

        public async Task<ImageUploaderResponse> UploadImageAsync(IFormFile profileImage, string fileName)
        {
            ImageUploaderResponse response = new ImageUploaderResponse();
            try
            {
                _logger.LogInformation("Starting Upload Image method...");
                var container = new BlobContainerClient(_storeConnectionString, _containerName);
                var createResponse = await container.CreateIfNotExistsAsync();
                if(createResponse != null && createResponse.GetRawResponse().Status == 201)
                {
                    await container.SetAccessPolicyAsync(PublicAccessType.BlobContainer);
                }
                var blob = container.GetBlobClient(fileName);
                await blob.DeleteIfExistsAsync(DeleteSnapshotsOption.IncludeSnapshots);

                using (var fileStream = profileImage.OpenReadStream())
                {
                    await blob.UploadAsync(fileStream, new BlobHttpHeaders { ContentType = profileImage.ContentType });
                }
                response.isUploaded = true;
                response.ImageUrl = blob.Uri.ToString();
            }
            catch (Exception ex)
            {
                response.isUploaded = false;
                _logger.LogError("An error occured while uploading the image", ex.Message);
            }
            return response;
        }

        public async Task<bool> DeleteImageFileAsync(string fileName)
        {
            bool isProfileDeleted = false;
            try
            {
                var container = new BlobContainerClient(_storeConnectionString, _containerName);
                var blob = container.GetBlobClient(fileName);
                var response = await blob.DeleteIfExistsAsync(DeleteSnapshotsOption.IncludeSnapshots);
                isProfileDeleted = response.Value;
            }
            catch (Exception ex)
            {
                _logger.LogError("An error occured while deleting the image", ex.Message);
            }
            return isProfileDeleted;
        }
    }
}
