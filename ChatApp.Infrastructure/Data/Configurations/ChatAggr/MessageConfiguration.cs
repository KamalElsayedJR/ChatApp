using ChatApp.Domain.Entities.ChatAggr;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatApp.Infrastructure.Data.Configurations.ChatAggr
{
    public class MessageConfiguration : IEntityTypeConfiguration<Message>
    {
        public void Configure(EntityTypeBuilder<Message> builder)
        {
            builder.HasKey(x => x.Id);
            builder.HasOne(m=>m.Sender).WithMany(s=>s.Messages).HasForeignKey(m=>m.SenderId);
            builder.HasOne(m=>m.Chat).WithMany(s=>s.Messages).HasForeignKey(m=>m.ChatId);
        }
    }
}
