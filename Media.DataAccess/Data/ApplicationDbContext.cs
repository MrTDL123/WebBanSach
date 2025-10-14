
using Microsoft.EntityFrameworkCore;
using System.Runtime.InteropServices;
using Media.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
namespace Meida.DataAccess.Data
{
    public class ApplicationDbContext : IdentityDbContext<TaiKhoan>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)//pass connection string từ program.cs cho DbContext
        {

        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<TaiKhoan>()
                .HasOne(kh => kh.KhachHang)
                .WithOne(tk => tk.TaiKhoan)
                .HasForeignKey<KhachHang>(kh => kh.TaiKhoanId);
        }

        //Create Table
        public DbSet<ChuDe> ChuDes { get; set; }
        public DbSet<Sach> Saches { get; set; }
        public DbSet<NhaXuatBan> NhaXuatBans { get; set; }
        public DbSet<TacGia> TacGias { get; set; }
        public DbSet<TaiKhoan> KhachHangs { get; set; }
        public DbSet<DonHang> DonHangs { get; set; }
        public DbSet<ChiTietDonHang> ChiTietDonHangs { get; set; }
        
    }
}
