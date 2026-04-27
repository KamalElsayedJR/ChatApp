using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatApp.Domain.Entities
{
    public class RefreshToken
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Token { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime ExpiredAt { get; set; }
        public bool IsRevoked { get; set; } = false;
        public string UserId { get; set; } = string.Empty;
        public User User { get; set; }
        private RefreshToken(){}
        public RefreshToken(string token, DateTime expiredAt, string userId)
        {
            Token = token;
            ExpiredAt = expiredAt;
            UserId = userId;
        }

    }
}
