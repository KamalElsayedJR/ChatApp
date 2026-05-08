using ChatApp.Domain.Entities.ChatAggr;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatApp.Application.Specification.ChatSpec
{
    public class ChatsForUserSpec : BaseSpecification<Chat>
    {
        public ChatsForUserSpec(string userId) : base(c => c.ChatParticipants.Any(cp => cp.UserId == userId))
        {
            Includes.Add(c => c.ChatParticipants);
            Includes.Add(c=>c.ChatParticipants.Select(cp=>cp.User));
        }
    }
}
