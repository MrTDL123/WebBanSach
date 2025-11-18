using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Media.DataAccess.Repository.IRepository
{
    public interface IRepository<T> where T : class
    {
        T? Get(Expression<Func<T, bool>> filter, string? includeProperties = null);
        Task<T> GetAsync(Expression<Func<T, bool>> filter, string? includeProperties = null);
        IEnumerable<T>? GetAll(string? includeProperties = null);
        Task<List<T>> GetAllReadOnlyAsync(string? includeProperties = null);
        IEnumerable<T>? GetRange(Expression<Func<T, bool>> filter, string? includeProperties = null);
        Task<List<T>> GetRangeAsync(Expression<Func<T, bool>>? filter = null, Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null, string? includeProperties = null);

        Task<List<T>> GetRangeReadOnlyAsync(Expression<Func<T, bool>> filter, string? includeProperties = null);
        void Add(T entity);
        void Remove(T entity);
        void RemoveRange(IEnumerable<T> entities);
        void AddRange(IEnumerable<T> entities);
        void Update(T entity);
        int Count(Expression<Func<T, bool>> filter);
        T? GetById(object id);
        Task<T?> GetByIdAsync(object id);
        Task<int> CountAsync(Expression<Func<T, bool>>? filter = null);
    }
}