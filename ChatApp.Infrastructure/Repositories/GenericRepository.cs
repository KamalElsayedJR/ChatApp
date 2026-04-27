using ChatApp.Application.Repositories;
using ChatApp.Application.Specification;
using ChatApp.Infrastructure.Data;
using ChatApp.Infrastructure.Helper;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace ChatApp.Infrastructure.Repositories
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        private readonly AppDbContext dbContext;

        public GenericRepository(AppDbContext dbContext)
        {
            this.dbContext = dbContext;
        }
        public async Task AddAsync(T entity)
        => await dbContext.Set<T>().AddAsync(entity);
        public async Task<bool> AnyAsync(Expression<Func<T, bool>> expression)
        =>await dbContext.Set<T>().AnyAsync(expression);

        public void Delete(T entity)
        => dbContext.Set<T>().Remove(entity);

        public async Task<T?> GetByIdAsync(string id)
        => await dbContext.Set<T>().FirstOrDefaultAsync(e => EF.Property<string>(e, "Id") == id);
        

        public Task<T?> GetOneWithSpecAsync(ISpecification<T> spec)
        => SpecificationEvaluator<T>.GetQuery(dbContext.Set<T>(), spec).FirstOrDefaultAsync();

        public Task<List<T>> GetAllWithSpec(ISpecification<T> spec)
        {
            return SpecificationEvaluator<T>.GetQuery(dbContext.Set<T>(), spec).ToListAsync();
        }
    }
}
