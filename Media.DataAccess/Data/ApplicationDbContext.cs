using Media.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;
using System.Runtime.InteropServices;
namespace Meida.DataAccess.Data
{
    public class ApplicationDbContext : IdentityDbContext<TaiKhoan>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)//pass connection string từ program.cs cho DbContext
        {

        }

        // ======================= DANH MỤC =======================
        public DbSet<Sach> Saches { get; set; }
        public DbSet<NhaXuatBan> NhaXuatBans { get; set; }
        public DbSet<ChuDe> ChuDes { get; set; }
        public DbSet<TacGia> TacGias { get; set; }

        // ======================= NGƯỜI DÙNG =======================
        public DbSet<TaiKhoan> TaiKhoans { get; set; }
        public DbSet<KhachHang> KhachHangs { get; set; }
        public DbSet<NhanVien> NhanViens { get; set; }
        public DbSet<QuanLy> QuanLys { get; set; }
        public DbSet<HauCan> HauCans { get; set; }
        public DbSet<KeToan> KeToans { get; set; }
        public DbSet<ChamSocKhachHang> ChamSocKhachHangs { get; set; }

        // ======================= NGHIỆP VỤ =======================
        public DbSet<GioHang> GioHangs { get; set; }
        public DbSet<ChiTietGioHang> ChiTietGioHangs { get; set; }

        public DbSet<DonHang> DonHangs { get; set; }
        public DbSet<ChiTietDonHang> ChiTietDonHangs { get; set; }

        public DbSet<HoaDon> HoaDons { get; set; }
        public DbSet<PhieuTraHang> PhieuTraHangs { get; set; }
        public DbSet<ChiTietTraHang> ChiTietTraHangs { get; set; }
        public DbSet<VanChuyen> VanChuyens { get; set; }
        public DbSet<DanhGiaSanPham> DanhGiaSanPhams { get; set; }
        public DbSet<LuotThichDanhGiaSanPham> LuotThichDanhGiaSanPhams { get; set; }
        public DbSet<DiaChiNhanHang> DiaChiNhanHangs{ get; set; }
        

        // ======================= CONFIG =======================
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // ========== CẤU HÌNH TPH CHO NHÂN VIÊN ==========
            modelBuilder.Entity<NhanVien>()
                .HasDiscriminator<string>("LoaiNhanVien")
                .HasValue<NhanVien>("NhanVien")
                .HasValue<QuanLy>("QuanLy")
                .HasValue<HauCan>("HauCan")
                .HasValue<KeToan>("KeToan");

            // ========== TÀI KHOẢN – KHÁCH HÀNG / NHÂN VIÊN ==========
            modelBuilder.Entity<TaiKhoan>()
                .HasOne(t => t.KhachHang)
                .WithOne(kh => kh.TaiKhoan)
                .HasForeignKey<KhachHang>(kh => kh.MaTaiKhoan)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<TaiKhoan>()
                .HasOne(t => t.NhanVien)
                .WithOne(nv => nv.TaiKhoan)
                .HasForeignKey<NhanVien>(nv => nv.MaTaiKhoan)
                .OnDelete(DeleteBehavior.Cascade);

            // ==========  NHÂN VIÊN – CHĂM SÓC KHÁCH HÀNG (1-N) ==========
            modelBuilder.Entity<NhanVien>()
                .HasMany(nv => nv.ChamSocKhachHangs)
                .WithOne(cs => cs.NhanVien)
                .HasForeignKey(cs => cs.MaNhanVien)
                .OnDelete(DeleteBehavior.Restrict);

            // ==========  KHÁCH HÀNG – CHĂM SÓC KHÁCH HÀNG (1-N) ==========
            modelBuilder.Entity<KhachHang>()
                .HasMany(kh => kh.ChamSocKhachHangs)
                .WithOne(cs => cs.KhachHang)
                .HasForeignKey(cs => cs.MaKhachHang)
                .OnDelete(DeleteBehavior.Restrict);

            // ========== KHÁCH HÀNG – GIỎ HÀNG (1-1) ==========
            modelBuilder.Entity<KhachHang>()
                .HasOne(kh => kh.GioHang)
                .WithOne(gh => gh.KhachHang)
                .HasForeignKey<GioHang>(gh => gh.MaKhachHang)
                .OnDelete(DeleteBehavior.Cascade);

            // ========== GIỎ HÀNG – CHI TIẾT GIỎ HÀNG (1-N) ==========
            modelBuilder.Entity<ChiTietGioHang>()
                .HasKey(ct => new { ct.MaGioHang, ct.MaSach });

            modelBuilder.Entity<GioHang>()
                .HasMany(gh => gh.ChiTietGioHangs)
                .WithOne(ct => ct.GioHang)
                .HasForeignKey(ct => ct.MaGioHang)
                .OnDelete(DeleteBehavior.Cascade);

            // ========== SÁCH – CHI TIẾT GIỎ HÀNG (N-1) ==========
            modelBuilder.Entity<Sach>()
                .HasMany(s => s.ChiTietGioHangs)
                .WithOne(ct => ct.Sach)
                .HasForeignKey(ct => ct.MaSach)
                .OnDelete(DeleteBehavior.Restrict);

            // ========== ĐƠN HÀNG – CHI TIẾT ĐƠN HÀNG (1-N) ==========
            modelBuilder.Entity<ChiTietDonHang>()
                .HasKey(ct => new { ct.MaDonHang, ct.MaSach });

            modelBuilder.Entity<DonHang>()
                .HasMany(dh => dh.ChiTietDonHangs)
                .WithOne(ct => ct.DonHang)
                .HasForeignKey(ct => ct.MaDonHang)
                .OnDelete(DeleteBehavior.Cascade);

            // ========== SÁCH – CHI TIẾT ĐƠN HÀNG (N:1) ==========
            modelBuilder.Entity<Sach>()
                .HasMany(s => s.ChiTietDonHangs)
                .WithOne(ct => ct.Sach)
                .HasForeignKey(ct => ct.MaSach)
                .OnDelete(DeleteBehavior.Restrict); // Mối quan hệ còn thiếu

            // ========== NHÂN VIÊN – ĐƠN HÀNG (1-N) ==========
            modelBuilder.Entity<NhanVien>()
                .HasMany(nv => nv.DonHangs)
                .WithOne(dh => dh.NhanVien)
                .HasForeignKey(dh => dh.MaNhanVien)
                .OnDelete(DeleteBehavior.Restrict);

            // ========== KHÁCH HÀNG – ĐƠN HÀNG (1-N) ==========
            modelBuilder.Entity<KhachHang>()
                .HasMany(kh => kh.DonHangs)
                .WithOne(dh => dh.KhachHang)
                .HasForeignKey(dh => dh.MaKhachHang)
                .OnDelete(DeleteBehavior.Restrict);

            // ========== ĐƠN HÀNG – HÓA ĐƠN (1-1) ==========
            modelBuilder.Entity<DonHang>()
                .HasOne(dh => dh.HoaDon)
                .WithOne(hd => hd.DonHang)
                .HasForeignKey<HoaDon>(hd => hd.MaDonHang)
                .OnDelete(DeleteBehavior.Cascade);

            // ========== PHIẾU TRẢ HÀNG - CHI TIẾT TRẢ HÀNG (1-N) ==========
            modelBuilder.Entity<ChiTietTraHang>()
                .HasKey(ct => new { ct.MaPhieuTraHang, ct.MaSach });

            modelBuilder.Entity<PhieuTraHang>()
                .HasMany(p => p.ChiTietTraHangs)
                .WithOne(ct => ct.PhieuTraHang)
                .HasForeignKey(ct => ct.MaPhieuTraHang)
                .OnDelete(DeleteBehavior.Cascade);

            // ==========  NHÂN VIÊN – PHIẾU TRẢ HÀNG (1-N) ==========
            modelBuilder.Entity<NhanVien>()
                .HasMany(nv => nv.PhieuTraHangs)
                .WithOne(pt => pt.NhanVien)
                .HasForeignKey(pt => pt.MaNhanVien)
                .OnDelete(DeleteBehavior.Restrict);

            // ========== VẬN CHUYỂN - ĐƠN HÀNG (1-1) ==========
            modelBuilder.Entity<DonHang>()
                .HasOne(dh => dh.VanChuyen)
                .WithOne(vc => vc.DonHang)
                .HasForeignKey<VanChuyen>(vc => vc.MaDonHang)
                .OnDelete(DeleteBehavior.Restrict);


            // ========== KHÁCH HÀNG - ĐỊA CHỈ NHẬN HÀNG (1-N) ==========
            modelBuilder.Entity<KhachHang>()
                .HasMany(kh => kh.DiaChiNhanHangs)
                .WithOne(dc => dc.KhachHang)
                .HasForeignKey(dc => dc.MaKhachHang)
                .OnDelete(DeleteBehavior.Restrict);

            // ========== ĐƠN HÀNG - ĐỊA CHỈ NHẬN HÀNG (n-1) ==========
            modelBuilder.Entity<DonHang>()
                .HasOne(dh => dh.DiaChiNhanHang)
                .WithMany(dc => dc.DonHangs)
                .HasForeignKey(dh => dh.MaDiaChi)
                .OnDelete(DeleteBehavior.Restrict);

            // ========== SÁCH - ĐÁNH GIÁ (1-N) ==========
            modelBuilder.Entity<Sach>()
                .HasMany(s => s.DanhGiaSanPhams)
                .WithOne(dg => dg.Sach)
                .HasForeignKey(dg => dg.MaSach)
                .OnDelete(DeleteBehavior.Restrict);

            // ========== KHÁCH HÀNG - ĐÁNH GIÁ (1-N) ==========
            modelBuilder.Entity<KhachHang>()
                .HasMany(kh => kh.DanhGiaSanPhams)
                .WithOne(dg => dg.KhachHang)
                .HasForeignKey(dg => dg.MaKhachHang)
                .OnDelete(DeleteBehavior.Restrict);

            // ========== CẤU HÌNH BẢNG "LƯỢT THÍCH" (Bảng Join N-N) ==========

            // 1. Đặt khóa chính tổng hợp (Composite Key)
            modelBuilder.Entity<LuotThichDanhGiaSanPham>()
                .HasKey(lt => new { lt.MaKhachHang, lt.MaDanhGia });

            // 2. Quan hệ: KHÁCH HÀNG -> LƯỢT THÍCH (1-N)
            modelBuilder.Entity<KhachHang>()
                .HasMany(kh => kh.LuotThichDanhGiaSanPhams)
                .WithOne(lt => lt.KhachHang)
                .HasForeignKey(lt => lt.MaKhachHang)
                .OnDelete(DeleteBehavior.Cascade);

            // 3. Quan hệ: ĐÁNH GIÁ -> LƯỢT THÍCH (1-N)
            modelBuilder.Entity<DanhGiaSanPham>()
                .HasMany(dg => dg.LuotThichDanhGiaSanPhams)
                .WithOne(lt => lt.DanhGiaSanPham)
                .HasForeignKey(lt => lt.MaDanhGia)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}