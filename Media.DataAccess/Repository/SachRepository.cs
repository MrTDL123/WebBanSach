using Media.DataAccess.Repository.IRepository;
using Media.Models;
using Meida.DataAccess.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
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

        public async Task<List<Sach>> LaySachTheoChuDe(int id)
        {
            var dsIdChuDe = new List<int>();
            await GetChildCategoryIds(id, dsIdChuDe);

            return await _db.Saches
                .Include(s => s.ChuDe)
                .Include(s => s.TacGia)
                .Where(s => dsIdChuDe.Contains(s.MaChuDe))
                .AsNoTracking()
                .ToListAsync();
        }

        private async Task GetChildCategoryIds(int parentId, List<int> dsIdChuDe)
        {
            dsIdChuDe.Add(parentId);

            var childCategories = await _db.ChuDes
                .Where(cd => cd.ParentId == parentId)
                .Select(cd => cd.MaChuDe)
                .ToListAsync();

            foreach (var childId in childCategories)
            {
                await GetChildCategoryIds(childId, dsIdChuDe);
            }
        }

        public void Update(Sach obj)
        {
            var objFromDb = _db.Saches.FirstOrDefault(u => u.MaSach == obj.MaSach);

            if (objFromDb != null)
            {
                objFromDb.TenSach = obj.TenSach;
                objFromDb.MoTa = obj.MoTa;
                objFromDb.GiaBan = obj.GiaBan;
                objFromDb.NgayCapNhap = DateTime.Now;
                objFromDb.SoLuong = obj.SoLuong;
                objFromDb.MaChuDe = obj.MaChuDe;
                objFromDb.MaTacGia = obj.MaTacGia;
                objFromDb.MaNhaXuatBan = obj.MaNhaXuatBan;

                if (obj.ImageUrl != null)
                {
                    objFromDb.ImageUrl = obj.ImageUrl;
                }
            }
        }
    }
}
