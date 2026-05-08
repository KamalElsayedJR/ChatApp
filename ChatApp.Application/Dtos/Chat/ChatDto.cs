using ChatApp.Domain.Entities.ChatAggr;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatApp.Application.Dtos.Chat
{
    public class ChatDto
    {
        public DateTime CreatedAt { get; set; }
        public ICollection<MessageDto> Messages { get; set; }

    }
}
