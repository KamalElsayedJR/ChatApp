using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatApp.Application.Dtos.Image
{
    public class UpdateProfilePictureRequest
    {
        public IFormFile File { get; set; }
    }
}
