using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatApp.Application.Dtos.Auth
{
    public class LoginResposnse
    {
        public string FullName { get;  set; }
        public string UserName { get;  set; }
        public string ProfilePictureURL { get;  set; } 
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }

    }
}
