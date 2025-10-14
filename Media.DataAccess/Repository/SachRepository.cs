using Media.DataAccess.Repository.IRepository;
using Media.Models;
using Meida.DataAccess.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Media.DataAccess.Repository
{
    public class SachRepository : Repository<Sach>, ISachRepository
    {
        private readonly ApplicationDbContext _db;
        public SachRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }
        public void Update(Sach obj)
        {
            var objFromDb = _db.Saches.FirstOrDefault(u => u.Id == obj.Id);

            if (objFromDb != null)
            {
                objFromDb.TenSach = obj.TenSach;
                objFromDb.MoTa = obj.MoTa;
                objFromDb.GiaBan = obj.GiaBan;
                objFromDb.NgayCapNhap = DateTime.Now;
                objFromDb.SoLuong = obj.SoLuong;
                objFromDb.ChuDeId = obj.ChuDeId;
                objFromDb.TacGiaId = obj.TacGiaId;
                objFromDb.NhaXuatBanId = obj.NhaXuatBanId;

                if (obj.ImageUrl != null)
                {
                    objFromDb.ImageUrl = obj.ImageUrl;
                }
            }
        }
    }
}
