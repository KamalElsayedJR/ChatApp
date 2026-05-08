using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatApp.Application.Dtos.Chat
{
    public class UserChatsDto
    {
        public string ChatId { get; set; }
        public string FullName { get; set; }
        public string UserName { get; set; }
        public string ProfilePictureURL { get; set; }
        public string? LastMessage { get; set; }
        public DateTime? LastMessageAt { get; set; }
    }
}
