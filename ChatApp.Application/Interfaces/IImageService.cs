using ChatApp.Domain.Common;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatApp.Application.Interfaces
{
    public interface IImageService
    {
        public Task<string?> UploadImageAsync(IFormFile File);
    }
}
