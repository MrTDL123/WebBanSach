using Media.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Media.DataAccess.Repository.IRepository
{
    public interface IChuDeRepository : IRepository<ChuDe>
    {
        void Update(ChuDe obj);
    }
}
