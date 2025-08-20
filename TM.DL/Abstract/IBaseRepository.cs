using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace TM.DAL.Abstract
{
    public interface IBaseRepository<T> where T : class
    {
        Task<T> GetAsync(
        Expression<Func<T, bool>> predicate = null,
        Func<IQueryable<T>, IQueryable<T>> includeFunc = null,
        bool asNoTracking = false);

        Task<List<T>> GetListAsync(
            Expression<Func<T, bool>> predicate = null,
            Func<IQueryable<T>, IQueryable<T>> includeFunc = null,
            bool asNoTracking = false);

        Task<T> GetByIdAsync(int id);
        Task AddAsync(T entity);
        Task UpdateAsync(T entity);
        Task DeleteAsync(T entity);
        Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate);
    }
}
