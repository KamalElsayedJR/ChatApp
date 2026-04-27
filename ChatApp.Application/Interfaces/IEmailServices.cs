using ChatApp.Application.Dtos.Mail;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatApp.Application.Interfaces
{
    public interface IEmailServices
    {
        void SendEmail(Email email);
    }
}
