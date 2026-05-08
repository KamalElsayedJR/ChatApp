using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatApp.Domain.Entities.ChatAggr
{
    public class ChatParticipant
    {
        public string UserId { get; set; }
        public User User { get; set; } 
        public string ChatId { get; set; }
        public Chat Chat { get; set; }
        public DateTime JoinedAt { get; set; } = DateTime.UtcNow;
    }
}
