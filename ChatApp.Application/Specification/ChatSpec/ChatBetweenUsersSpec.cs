using ChatApp.Domain.Entities.ChatAggr;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatApp.Application.Specification.ChatSpec
{
    public class ChatBetweenUsersSpec : BaseSpecification<Chat>
    {
        public ChatBetweenUsersSpec(string senderId, string receiverId)
            :base(
            c => c.ChatParticipants.Any(cp => cp.UserId == senderId) &&
                 c.ChatParticipants.Any(cp => cp.UserId == receiverId))
        {
            Includes.Add(c => c.ChatParticipants);
        }
    }
}
