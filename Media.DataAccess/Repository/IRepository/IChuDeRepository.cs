using Media.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Media.DataAccess.Repository.IRepository
{
    public interface IChuDeRepository : IRepository<ChuDe>
    {
        Task<List<ChuDe>> GetAllReadOnlyAsync(string? includeProperties = null);
        Task<List<ChuDe>> GetRangeReadOnly(Expression<Func<ChuDe, bool>> filter, string? includeProperties = null);
        void Update(ChuDe obj);
    }
}
