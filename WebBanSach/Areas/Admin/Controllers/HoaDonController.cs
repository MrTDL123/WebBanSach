using Media.Areas.Admin.Controllers;
using Media.Models;
using Meida.DataAccess.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace ProjectCuoiKi.Areas.Admin.Controllers
{
    public class HoaDonController : AdminController
    {
        private readonly ApplicationDbContext _context;

        public HoaDonController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: HoaDon/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var hoaDon = await _context.HoaDons
                .Include(h => h.DonHang)
                    .ThenInclude(d => d.KhachHang)
                .Include(h => h.DonHang)
                    .ThenInclude(d => d.VanChuyen)
                .Include(h => h.DonHang)
                    .ThenInclude(d => d.ChiTietDonHangs)
                        .ThenInclude(ct => ct.Sach)
                .Include(h => h.DonHang)
                    .ThenInclude(d => d.NhanVien)
                .FirstOrDefaultAsync(m => m.MaHoaDon == id);

            if (hoaDon == null)
            {
                return NotFound();
            }

            // Tính toán lại tổng tiền nếu cần
            if (hoaDon.TongTien == null || hoaDon.TongTien == 0)
            {
                hoaDon.TongTien = await TinhTongTienHoaDon(hoaDon.MaHoaDon);
            }

            return View(hoaDon);
        }

        private async Task<decimal> TinhTongTienHoaDon(int maHoaDon)
        {
            var hoaDon = await _context.HoaDons
                .Include(h => h.DonHang)
                    .ThenInclude(d => d.ChiTietDonHangs)
                .FirstOrDefaultAsync(h => h.MaHoaDon == maHoaDon);

            if (hoaDon == null) return 0;

            decimal tongTien = hoaDon.DonHang.Total;

            // Áp dụng VAT nếu có
            if (hoaDon.VAT > 0)
            {
                tongTien += tongTien * hoaDon.VAT / 100;
            }

            // Cộng thêm phí vận chuyển nếu có
            var vanChuyen = await _context.VanChuyens
                .FirstOrDefaultAsync(v => v.MaDonHang == hoaDon.DonHang.MaDonHang);

            if (vanChuyen != null)
            {
                tongTien += vanChuyen.PhiVanChuyen;
            }

            return tongTien;
        }
    }
}