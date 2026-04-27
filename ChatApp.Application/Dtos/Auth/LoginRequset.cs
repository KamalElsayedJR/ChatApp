using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatApp.Application.Dtos.Auth
{
    public class LoginRequset
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }
}
