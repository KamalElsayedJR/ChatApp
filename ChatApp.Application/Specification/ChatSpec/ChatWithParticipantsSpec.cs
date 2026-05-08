using ChatApp.Domain.Entities.ChatAggr;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatApp.Application.Specification.ChatSpec
{
    public class ChatWithParticipantsSpec :BaseSpecification<Chat>
    {
        public ChatWithParticipantsSpec(string chatId) : base(c => c.Id == chatId)
        {
            Includes.Add(c => c.ChatParticipants);
            Includes.Add(c => c.Messages);
        }
    }
}
