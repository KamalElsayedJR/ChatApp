using ChatApp.Application.Repositories;
using ChatApp.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatApp.Infrastructure.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly Hashtable _repo = new Hashtable();
        private readonly AppDbContext _dbContext;
        public UnitOfWork(AppDbContext dbContext)
        {
            _dbContext = dbContext;
            UserRepository = new UserRepository(_dbContext);
            RefreshTokenRepository = new RefreshTokenRepository(_dbContext);
        }

        public IUserRepository UserRepository { get; }

        public IRefreshTokenRepository RefreshTokenRepository { get; }

        public async ValueTask DisposeAsync()
        => await _dbContext.DisposeAsync();
        public IGenericRepository<T> Repository<T>() where T : class
        {
            var type = typeof(T).Name;
            if (!_repo.ContainsKey(type))
            {
                var repo = new GenericRepository<T>(_dbContext);
                _repo.Add(type, repo);
            }
            return (IGenericRepository<T>)_repo[type];
        }

        public async Task<int> SaveChangesAsync()
        => await _dbContext.SaveChangesAsync();
    }
}
