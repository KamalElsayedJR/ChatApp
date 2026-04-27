using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatApp.Application.Dtos
{
    public class UserDto
    {
        public string FullName { get;  set; } 
        public string UserName { get;  set; } 
        public string Email { get;  set; } 
        public string ProfilePictureURL { get;  set; } 
        public string Bio { get;  set; } 
        public bool IsEmailConfirmed { get;  set; }
        public bool IsActive { get;  set; } 
        public DateTime CreatedAt { get;  set; } 
        public DateTime UpdatedAt { get;  set; } 
    }
}
