using ChatApp.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatApp.Infrastructure.Data.Configurations
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.HasKey(u => u.Id);
            builder.Property(u => u.Id).HasMaxLength(50);
            builder.Property(u => u.Email).IsRequired().HasMaxLength(255);
            builder.HasIndex(u => u.Email).IsUnique();
            builder.Property(u => u.UserName).IsRequired().HasMaxLength(100);
            builder.HasIndex(u => u.UserName).IsUnique();
            builder.Property(u => u.HashedPassword).IsRequired();
            builder.Property(u => u.FullName).IsRequired().HasMaxLength(150);
            builder.Property(u => u.IsActive).HasDefaultValue(true);
            builder.Property(u => u.Bio).HasMaxLength(500);
            builder.Property(u => u.IsEmailConfirmed) .HasDefaultValue(false);
            builder.Property(u => u.CreatedAt).IsRequired();
            builder.Property(u => u.UpdatedAt) .IsRequired();
        }
    }
}
