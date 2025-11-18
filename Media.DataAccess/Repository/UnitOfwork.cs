using Media.DataAccess.Repository.IRepository;
using Meida.DataAccess.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Media.DataAccess.Repository
{
    public class UnitOfwork : IUnitOfWork
    {
        private readonly ApplicationDbContext _db;
        //Add Table
        public IChuDeRepository ChuDes { get; private set; }
        public ISachRepository Saches { get; private set; }
        public IKhachHangRepository KhachHangs { get; private set; }
        public IGioHangRepository GioHangs { get; private set; }
        public IChiTietGioHangRepository ChiTietGioHangs { get; private set; }
        public IDiaChiNhanHangRepository DiaChiNhanHangs { get; private set; }
        public IDonHangRepository DonHangs { get; private set; }
        public IHoaDonRepository HoaDons { get; private set; }
        public IVanChuyenRepository VanChuyens { get; private set; }
        public IDanhGiaSanPhamRepository DanhGiaSanPhams { get; private set; }
        public ILuotThichDanhGiaSanPhamRepository LuotThichDanhGiaSanPhams { get; private set; }
        public UnitOfwork(ApplicationDbContext db)
        {
            _db = db;
            ChuDes = new ChuDeRepository(_db);
            Saches = new SachRepository(_db);
            KhachHangs = new KhachHangRepository(_db);
            GioHangs = new GioHangRepository(_db);
            ChiTietGioHangs = new ChiTietGioHangRepository(_db);
            DiaChiNhanHangs = new DiaChiNhanHangRepository(_db);
            DonHangs = new DonHangRepository(_db);
            HoaDons = new HoaDonRepository(_db);
            VanChuyens = new VanChuyenRepository(_db);
            DanhGiaSanPhams = new DanhGiaSanPhamRepository(_db);
            LuotThichDanhGiaSanPhams = new LuotThichDanhGiaSanPhamRepository(db);
        }
        public void Save()
        {
            _db.SaveChanges();
        }

        public async Task SaveAsync()
        {
            await _db.SaveChangesAsync();
        }
    }
}