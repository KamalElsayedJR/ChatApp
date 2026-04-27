using ChatApp.Application.Dtos.Mail;
using ChatApp.Application.Interfaces;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Options;
using MimeKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatApp.Infrastructure.Services
{
    public class EmailServices : IEmailServices
    {
        private readonly MailSettings _settings;

        public EmailServices(IOptions<MailSettings> settings)
        {
            _settings = settings.Value;
        }
        public void SendEmail(Email email)
        {
            var Mail = new MimeMessage
            {
                Sender = MailboxAddress.Parse(_settings.Email),
                Subject = email.Subject,
                To = { MailboxAddress.Parse(email.To) },
                Body = new TextPart(MimeKit.Text.TextFormat.Html)
                {
                    Text = email.Body
                },
                From = { new MailboxAddress(_settings.DisplayName, _settings.Email) }
            };

            using var smpt = new SmtpClient();
            smpt.Connect(_settings.Host, _settings.Port,MailKit.Security.SecureSocketOptions.StartTls);
            smpt.Authenticate(_settings.Email, _settings.Password);
            smpt.Send(Mail);
            smpt.Disconnect(true);
        }
    }
}
