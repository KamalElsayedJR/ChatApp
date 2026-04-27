using ChatApp.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatApp.Application.Dtos.Mail
{
    public class SendOtpRequest
    {
        public string Email { get; set; } = null!;
    }
}
