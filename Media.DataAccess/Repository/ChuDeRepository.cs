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
    public class ChuDeRepository : Repository<ChuDe>, IChuDeRepository
    {
        private readonly ApplicationDbContext _db;
        public ChuDeRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public async Task<List<ChuDe>> GetAllReadOnlyAsync(string? includeProperties = null)
        {
            IQueryable<ChuDe> query = dbSet;

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

        public async Task<List<ChuDe>> GetRangeReadOnly(Expression<Func<ChuDe, bool>> filter, string? includeProperties = null)
        {
            IQueryable<ChuDe> query = dbSet;
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

        public void Update(ChuDe obj)
        {
            _db.ChuDes.Update(obj);
        }
    }
}