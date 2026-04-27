using ChatApp.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatApp.Domain.Entities
{
    public class UserOtp
    {
        public int Id { get; set; }
        public int Otp { get; set; }
        public OtpType OtpType { get; set; }
        public DateTime ExpirationTime { get; set; } = DateTime.UtcNow.AddMinutes(3);
        public string UserId { get; set; }
        public User User { get; set; }
        private UserOtp() { }
        public UserOtp(int otp, OtpType otpType, string userId)
        {
            Otp = otp;
            OtpType = otpType;
            UserId = userId;
        }
    }
}
