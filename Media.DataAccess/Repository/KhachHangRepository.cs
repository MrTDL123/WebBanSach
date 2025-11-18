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
    public class KhachHangRepository : Repository<KhachHang>, IKhachHangRepository
    {
        private readonly ApplicationDbContext _db;
        public KhachHangRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }
    }
}
