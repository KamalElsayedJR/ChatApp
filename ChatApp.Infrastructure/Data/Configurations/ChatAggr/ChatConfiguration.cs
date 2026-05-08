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
    public class ChatConfiguration : IEntityTypeConfiguration<Chat>
    {
        public void Configure(EntityTypeBuilder<Chat> builder)
        {
            builder.HasKey(x => x.Id);
            builder.HasMany(c => c.ChatParticipants).WithOne(c=>c.Chat).HasForeignKey(c => c.ChatId);
            builder.HasMany(c => c.Messages).WithOne(m => m.Chat).HasForeignKey(m => m.ChatId);
        }
    }
}
