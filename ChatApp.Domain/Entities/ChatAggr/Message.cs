using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatApp.Domain.Entities.ChatAggr
{
    public class Message
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string ChatId { get; set; }
        public Chat Chat { get; set; }
        public string SenderId { get; set; }
        public User Sender { get; set; }
        public string Content { get; set; }
        public DateTime SentAt { get; set; } = DateTime.UtcNow;
        public bool IsRead { get; set; }
        public DateTime? ReadAt { get; set; }
    }
}
