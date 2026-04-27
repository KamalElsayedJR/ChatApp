using ChatApp.Application.Dtos.Image;
using ChatApp.Application.Interfaces;
using ChatApp.Domain.Common;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatApp.Infrastructure.Services
{
    public class CloudinaryServices : IImageService
    {
        public CloudinarySettings _settings;
        public CloudinaryServices(IOptions<CloudinarySettings> Settings) 
        { 
            _settings = Settings.Value;
        }
        public async Task<string?> UploadImageAsync(IFormFile file)
        {
            var cloudinary = new CloudinaryDotNet.Cloudinary(new CloudinaryDotNet.Account(_settings.CloudName, _settings.ApiKey, _settings.ApiSecret));
            using var stream = file.OpenReadStream();
            var uploadParams = new CloudinaryDotNet.Actions.ImageUploadParams
            {
                File = new CloudinaryDotNet.FileDescription(file.FileName, stream)
            };
            var result = await cloudinary.UploadAsync(uploadParams);
            if (result.Error != null)
            {
                return null;
            }
            return result.SecureUrl.ToString();
        }
    }
}
