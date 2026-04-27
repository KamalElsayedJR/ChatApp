using ChatApp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace ChatApp.Application.Interfaces
{
    public interface IJWTServices
    {
        public string GenerateJWTAccessToken(User user);
        public ClaimsPrincipal GetPrincipalFromExpiredToken(string expiredToken);
    }
}
