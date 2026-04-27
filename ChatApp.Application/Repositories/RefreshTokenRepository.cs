using ChatApp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatApp.Application.Repositories
{
    public interface IRefreshTokenRepository
    {
        Task<RefreshToken?> GetRefreshTokenForUserAsync(string UserId);
    }
}
