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
    public class NhaXuatBanRepository : Repository<NhaXuatBan>, INhaXuatBanRepository
    {
        private readonly ApplicationDbContext _db;
        public NhaXuatBanRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public void Update(NhaXuatBan obj)
        {
            var objFromDb = _db.NhaXuatBans.FirstOrDefault(tg => tg.MaNhaXuatBan == obj.MaNhaXuatBan);

            if (objFromDb != null)
            {
                objFromDb.TenNXB = obj.TenNXB;
            }
        }
    }
}
