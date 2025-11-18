using Media.DataAccess.Repository.IRepository;
using Media.Models;
using Meida.DataAccess.Data;
using System;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Media.DataAccess.Repository
{
    public class DiaChiNhanHangRepository : Repository<DiaChiNhanHang>, IDiaChiNhanHangRepository
    {
        private readonly ApplicationDbContext _db;
        public DiaChiNhanHangRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }
    }
}