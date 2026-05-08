using ChatApp.Domain.Entities.ChatAggr;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatApp.Infrastructure.Data.Configurations.ChatAggr
{
    public class ChatParticipantConfiguration : IEntityTypeConfiguration<ChatParticipant>
    {
        public void Configure(EntityTypeBuilder<ChatParticipant> builder)
        {
            builder.HasKey(cp => new { cp.UserId,cp.ChatId});
            builder.HasOne(cp => cp.User).WithMany(u=>u.ChatParticipants).HasForeignKey(cp => cp.UserId);
            builder.HasOne(cp=>cp.Chat).WithMany(c => c.ChatParticipants).HasForeignKey(c=>c.ChatId);
        }
    }
}
