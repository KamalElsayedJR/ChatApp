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
    public class UserOtpConfiguration : IEntityTypeConfiguration<UserOtp>
    {
        public void Configure(EntityTypeBuilder<UserOtp> builder)
        {
            builder.HasKey(uo => uo.Id);
            builder.Property(uo => uo.Otp)
                .IsRequired()
                .HasMaxLength(6);
            builder.Property(uo => uo.ExpirationTime).IsRequired();
            builder.HasOne(uo => uo.User).WithMany(u => u.UserOtps).HasForeignKey(uo => uo.UserId);

        }
    }
}
