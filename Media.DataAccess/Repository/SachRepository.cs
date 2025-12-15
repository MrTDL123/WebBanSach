using Media.DataAccess.Repository.IRepository;
using Media.Models;
using Meida.DataAccess.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Caching.Memory;
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
        private readonly IMemoryCache _cache;
        public SachRepository(ApplicationDbContext db, IMemoryCache cache) : base(db)
        {
            _db = db;
            _cache = cache;
        }

        public IQueryable<Sach> LaySachTheoChuDe(int? id)
        {
            var dsIdChuDe = new List<int?>();
            GetChildCategoryIds(id, dsIdChuDe);

            return _db.Saches
                .Include(s => s.ChuDe)
                .Include(s => s.TacGia)
                .Include(s => s.NhaXuatBan)
                .Where(s => dsIdChuDe.Contains(s.MaChuDe))
                .OrderBy(s => s.TenSach)
                .AsNoTracking();
        }


        private void GetChildCategoryIds(int? parentId, List<int?> dsIdChuDe)
        {
            dsIdChuDe.Add(parentId);

            var childCategories = _db.ChuDes
                .Where(cd => cd.ParentId == parentId)
                .Select(cd => cd.MaChuDe)
                .ToList();

            foreach (var childId in childCategories)
            {
                GetChildCategoryIds(childId, dsIdChuDe);
            }
        }


        public async Task<IEnumerable<Sach>> LaySachBanChay(int? days, int? sachCount)
        {
            int actualDays = days ?? 7;
            int actualCount = sachCount ?? 7;
            
            string cacheKey = $"TopSellers_{actualDays}_{actualCount}"; //Đảm bảo key là độc nhất
            DateTime daysAgo = DateTime.Now.AddDays(-actualDays);

            List<Sach> topSellers = await _cache.GetOrCreateAsync(cacheKey, async entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(4);

                var topSellersQuery = _db.ChiTietDonHangs.
                    AsNoTracking()
                    .Where(ctdh => ctdh.DonHang.DaThanhToan == true &&
                                ctdh.DonHang.NgayTao >= daysAgo)
                    .GroupBy(cthd => cthd.MaSach)
                    .Select(g => new
                    {
                        MaSach = g.Key,
                        TongSoLuong = g.Sum(ctdh => ctdh.SoLuong)
                    })
                    .OrderByDescending(ctdh => ctdh.TongSoLuong)
                    .Take(sachCount ?? 6);

                var result = await topSellersQuery
                                .Join(
                                    _db.Saches.AsNoTracking(),
                                    top => top.MaSach,
                                    sach => sach.MaSach,
                                    (top, sach) => sach)
                                .Include(s => s.TacGia)
                                .ToListAsync();
                return result;
            });


            return topSellers;
        }
        public void Update(Sach obj)
        {
            var objFromDb = _db.Saches.FirstOrDefault(u => u.MaSach == obj.MaSach);

            if (objFromDb != null)
            {
                objFromDb.TenSach = obj.TenSach;
                objFromDb.MoTa = obj.MoTa;
                objFromDb.GiaBan = obj.GiaBan;
                objFromDb.NgayCapNhat = DateTime.Now;
                objFromDb.SoLuong = obj.SoLuong;
                objFromDb.MaChuDe = obj.MaChuDe;
                objFromDb.MaTacGia = obj.MaTacGia;
                objFromDb.MaNhaXuatBan = obj.MaNhaXuatBan;

                if (obj.AnhBiaChinh != null)
                {
                    objFromDb.AnhBiaChinh = obj.AnhBiaChinh;
                }
            }
        }
    }
}