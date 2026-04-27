using ChatApp.Application.Repositories;
using ChatApp.Domain.Entities;
using ChatApp.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatApp.Infrastructure.Repositories
{
    public class RefreshTokenRepository : IRefreshTokenRepository
    {
        private readonly AppDbContext _dbContext;

        public RefreshTokenRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<RefreshToken?> GetRefreshTokenForUserAsync(string UserId)
        => await _dbContext.RefreshTokens.FirstOrDefaultAsync(rt => rt.UserId == UserId);
        
    }
}
