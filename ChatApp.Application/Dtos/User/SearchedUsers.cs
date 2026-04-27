using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatApp.Application.Dtos.User
{
    public class SearchedUsers
    {
        public string FullName { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string ProfilePictureURL { get; set; }
        public string Bio { get; set; }
    }
}
