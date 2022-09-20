using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using User.API.Models;

namespace User.API.Infrastructure.Services.ImageHandlerService.Interface
{
    public interface IHandleImage
    {
        Task<ImageUploaderResponse> UploadImageAsync(IFormFile profileImage, string fileName);
        Task<bool> DeleteImageFileAsync(string fileName);
    }
}
