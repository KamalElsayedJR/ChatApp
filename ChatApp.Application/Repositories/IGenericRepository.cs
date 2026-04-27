using ChatApp.Application.Specification;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace ChatApp.Application.Repositories
{
    public interface IGenericRepository<T>
    {
        Task AddAsync(T entity);
        Task<bool> AnyAsync(Expression<Func<T,bool>> expression);

        void Delete(T entity);
        Task<T?> GetByIdAsync(string id);

        #region With Spec
        Task<T?> GetOneWithSpecAsync(ISpecification<T> spec);
        Task<List<T>> GetAllWithSpec(ISpecification<T> spec);
        #endregion
    }
}
