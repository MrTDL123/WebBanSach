using Media.DataAccess.Repository.IRepository;
using Media.Models;
using Meida.DataAccess.Data;
using System;
using System.Collections.Generic;
using System.Linq;
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

        public void Update(ChuDe obj)
        {
            _db.ChuDes.Update(obj);
        }
    }
}
