using ChatApp.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatApp.Application.Dtos.Auth
{
    public class VerifyOtpRequest
    {
        public string Email { get; set; }
        public int Otp { get; set; } 
    }
}
