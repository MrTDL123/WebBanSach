
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

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<ChiTietDonHang>().HasKey(c => new { c.MaChiTietDonHang, c.MaSach });
            modelBuilder.Entity<PhieuNhapKhoChiTiet>().HasKey(p => new { p.MaPhieuNhapKho, p.MaSach });


            // 1-1 DonDatHang - HoaDon (HoaDon.MaDonDatHang unique)
            modelBuilder.Entity<HoaDon>()
                        .HasOne(h => h.DonHang)
                        .WithOne(d => d.HoaDon)
                        .HasForeignKey<HoaDon>(h => h.MaDonHang);

            // TPH configuration for NhanVien and derived types (QuanLy, KeToan, HauCan)
            modelBuilder.Entity<NhanVien>()
                        .HasDiscriminator<string>("LoaiNhanVien")
                        .HasValue<NhanVien>("NhanVien")
                        .HasValue<QuanLy>("QuanLy")
                        .HasValue<KeToan>("KeToan")
                        .HasValue<HauCan>("HauCan");


            modelBuilder.Entity<TaiKhoan>()
                        .HasOne(tk => tk.KhachHang)
                        .WithOne(kh => kh.TaiKhoan)
                        .HasForeignKey<KhachHang>(kh => kh.MaTaiKhoan);


            modelBuilder.Entity<TaiKhoan>()
                        .HasOne(tk => tk.NhanVien)
                        .WithOne(nv => nv.TaiKhoan)
                        .HasForeignKey<NhanVien>(nv => nv.MaTaiKhoan);

            modelBuilder.Entity<DonHang>()
                        .HasOne(d => d.NhanVien)
                        .WithMany(n => n.DonHangs)
                        .HasForeignKey(d => d.MaNhanVien)
                        .OnDelete(DeleteBehavior.Restrict);


            modelBuilder.Entity<DonHang>()
                        .HasOne(d => d.KhachHang)
                        .WithMany(k => k.DonHangs)
                        .HasForeignKey(d => d.MaKhachHang);
        }

        //Create Table
        public DbSet<KhachHang> KhachHangs { get; set; }
        public DbSet<NhanVien> NhanViens { get; set; }
        public DbSet<TaiKhoan> TaiKhoans { get; set; }
        public DbSet<Sach> Saches { get; set; }
        public DbSet<TacGia> TacGias { get; set; }
        public DbSet<NhaXuatBan> NhaXuatBans { get; set; }
        public DbSet<ChuDe> ChuDes { get; set; }
        public DbSet<DonHang> DonHangs { get; set; }
        public DbSet<ChiTietDonHang> ChiTietDonHangs { get; set; }
        public DbSet<HoaDon> HoaDons { get; set; }
        public DbSet<PhieuNhapKho> PhieuNhapKhos { get; set; }
        public DbSet<PhieuNhapKhoChiTiet> PhieuNhapKhoChiTiets { get; set; }
        public DbSet<KiemKeSanPham> KiemKeSanPhams { get; set; }
        public DbSet<PhanHoiKhachHang> PhanHoiTuKhachHangs { get; set; }
        public DbSet<ChamSocKhachHang> ChamSocKhachHangs { get; set; }
    }
}
