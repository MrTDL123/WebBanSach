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
    public class YeuThichRepository : Repository<YeuThich>, IYeuThichRepository
    {
        private readonly ApplicationDbContext _db;
        public YeuThichRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }
    }
}
