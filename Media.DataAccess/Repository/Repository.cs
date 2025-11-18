using Media.DataAccess.Repository.IRepository;
using Media.Models;
using Meida.DataAccess.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Media.DataAccess.Repository
{
    public class Repository<T> : IRepository<T> where T : class
    {
        private readonly ApplicationDbContext _db;
        internal DbSet<T> dbSet;//Đại diện 1 bảng trong Database
        public Repository(ApplicationDbContext db)
        {
            _db = db;
            this.dbSet = _db.Set<T>();
        }
        public void Add(T entity)
        {
            dbSet.Add(entity);
        }

        public T? Get(Expression<Func<T, bool>> filter, string? includeProperties = null)
        {
            IQueryable<T> query = dbSet;
            query = dbSet.Where(filter);
            if (!string.IsNullOrEmpty(includeProperties))
            {
                foreach (var includeProp in includeProperties
                    .Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(includeProp);
                }
            }
            return query.FirstOrDefault();
        }

        public async Task<T> GetAsync(Expression<Func<T, bool>> filter, string? includeProperties = null)
        {
            IQueryable<T> query = dbSet;

            // Luôn luôn phải có filter cho GetAsync (để lấy 1)
            query = query.Where(filter);

            if (!string.IsNullOrEmpty(includeProperties))
            {
                foreach (var includeProp in includeProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(includeProp);
                }
            }

            // Trả về đối tượng đầu tiên tìm thấy (hoặc null) một cách bất đồng bộ
            return await query.FirstOrDefaultAsync();
        }

        public IEnumerable<T>? GetRange(Expression<Func<T, bool>> filter, string? includeProperties = null)
        {
            IQueryable<T> query = dbSet;
            query = dbSet.Where(filter);
            if (!string.IsNullOrEmpty(includeProperties))
            {
                foreach (var includeProp in includeProperties
                    .Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(includeProp);
                }
            }
   
            return query.ToList();
        }
        public async Task<List<T>> GetRangeAsync(Expression<Func<T, bool>>? filter = null, Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null, string? includeProperties = null)
        {
            IQueryable<T> query = dbSet;

            // 1. Áp dụng Filter (nếu có)
            if (filter != null)
            {
                query = query.Where(filter);
            }

            // 2. Áp dụng Include (nếu có)
            if (!string.IsNullOrEmpty(includeProperties))
            {
                // Tách chuỗi "VanChuyen,ChiTietDonHangs.Sach"
                foreach (var includeProp in includeProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(includeProp);
                }
            }

            // 3. Áp dụng Sắp xếp (nếu có)
            if (orderBy != null)
            {
                query = orderBy(query);
            }

            // 4. Lấy danh sách (List) một cách bất đồng bộ
            return await query.ToListAsync();
        }

        public async Task<List<T>> GetRangeAsync(Expression<Func<T, bool>>? filter = null, Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null, string? includeProperties = null)
        {
            IQueryable<T> query = dbSet;

            // 1. Áp dụng Filter (nếu có)
            if (filter != null)
            {
                query = query.Where(filter);
            }

            // 2. Áp dụng Include (nếu có)
            if (!string.IsNullOrEmpty(includeProperties))
            {
                // Tách chuỗi "VanChuyen,ChiTietDonHangs.Sach"
                foreach (var includeProp in includeProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(includeProp);
                }
            }

            // 3. Áp dụng Sắp xếp (nếu có)
            if (orderBy != null)
            {
                query = orderBy(query);
            }

            // 4. Lấy danh sách (List) một cách bất đồng bộ
            return await query.ToListAsync();
        }

        public IEnumerable<T>? GetAll(string? includeProperties = null)
        {
            IQueryable<T> query = dbSet;
            if (!string.IsNullOrEmpty(includeProperties))
            {
                foreach (var includeProp in includeProperties
                    .Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(includeProp);
                }
            }
            return query;
        }

        public async Task<List<T>> GetAllReadOnlyAsync(string? includeProperties = null)
        {
            IQueryable<T> query = dbSet;

            if (!string.IsNullOrEmpty(includeProperties))
            {
                foreach (var includeProp in includeProperties
                    .Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(includeProp);
                }
            }

            //AsNoTracking để báo EF rằng chỉ lấy danh sách để đọc
            return await query.AsNoTracking().ToListAsync();
        }

        public async Task<List<T>> GetRangeReadOnlyAsync(Expression<Func<T, bool>> filter, string? includeProperties = null)
        {
            IQueryable<T> query = dbSet;
            query = dbSet.Where(filter);
            if (!string.IsNullOrEmpty(includeProperties))
            {
                foreach (var includeProp in includeProperties
                    .Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(includeProp);
                }
            }

            return await query.AsNoTracking().ToListAsync();
        }

        public void Remove(T entity)
        {
            dbSet.Remove(entity);
        }

        public void RemoveRange(IEnumerable<T> entities)
        {
            dbSet.RemoveRange(entities); 
        }

        public void AddRange(IEnumerable<T> entities)
        {
            dbSet.AddRange(entities);
        }

        public void Update(T entity)
        {
            dbSet.Update(entity);
        }

        public int Count(Expression<Func<T, bool>> filter)
        {
            return dbSet.Count(filter);
        }

        public T GetById(object id)
        {
            return dbSet.Find(id);
        }

        public async Task<T?> GetByIdAsync(object id)
        {
            // dbSet là 'DbSet<DiaChiNhanHang>' đã được kế thừa từ lớp Repository cha
            // FindAsync là cách tối ưu nhất để tìm bằng Khóa Chính
            return await dbSet.FindAsync(id);
        }

        public async Task<int> CountAsync(Expression<Func<T, bool>>? filter = null)
        {
            if (filter != null)
            {
                return await dbSet.CountAsync(filter);
            }
            return await dbSet.CountAsync();
        }
    }
}