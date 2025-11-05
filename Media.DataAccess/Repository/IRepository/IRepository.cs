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
        IEnumerable<T>? GetAll(string? includeProperties = null);
        Task<List<T>> GetAllReadOnlyAsync(string? includeProperties = null);
        IEnumerable<T>? GetRange(Expression<Func<T, bool>> filter, string? includeProperties = null);
        Task<List<T>> GetRangeReadOnlyAsync(Expression<Func<T, bool>> filter, string? includeProperties = null);
        void Add(T entity);
        void Remove(T entity);
        void RemoveRange(IEnumerable<T> entity);
    }
}
