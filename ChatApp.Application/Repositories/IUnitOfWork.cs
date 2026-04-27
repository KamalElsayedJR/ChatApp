using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatApp.Application.Repositories
{
    public interface IUnitOfWork : IAsyncDisposable
    {
        Task<int> SaveChangesAsync();
        IGenericRepository<T> Repository<T>() where T : class;
        public IUserRepository UserRepository { get;}
        public IRefreshTokenRepository RefreshTokenRepository { get; }

    }
}
