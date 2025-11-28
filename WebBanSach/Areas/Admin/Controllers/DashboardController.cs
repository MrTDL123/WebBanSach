using Media.Models;
using Meida.DataAccess.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectCuoiKi.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class DashboardController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DashboardController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetData(int days = 30)
        {
            try
            {
                // SỬA: Dùng DateTime.UtcNow thay vì DateTime.Now
                var startDate = DateTime.UtcNow.Date.AddDays(-days);

                // =========================
                // 1. KPI Tổng quan
                // =========================
                var kpiData = new
                {
                    sach = await _context.Saches.CountAsync(),
                    user = await _context.TaiKhoans.CountAsync(),
                    revenue = await _context.DonHangs
                        .Where(x => x.DaThanhToan && x.NgayTao >= startDate)
                        .SumAsync(x => (decimal?)x.Total) ?? 0,
                    order = await _context.DonHangs
                        .Where(x => x.NgayTao >= startDate)
                        .CountAsync()
                };

                // =========================
                // 2. Doanh thu theo ngày - ĐÃ SỬA
                // =========================
                var doanhThuList = await _context.DonHangs
                    .Where(x => x.DaThanhToan && x.NgayTao >= startDate)
                    .Select(x => new { x.NgayTao, x.Total })
                    .ToListAsync();

                var doanhThuRaw = doanhThuList
                    .GroupBy(x => x.NgayTao.ToLocalTime().Date)
                    .Select(g => new { Ngay = g.Key, Total = g.Sum(x => x.Total) })
                    .OrderBy(g => g.Ngay)
                    .ToList();

                var labels = doanhThuRaw.Select(x => x.Ngay.ToString("dd/MM")).ToArray();
                var doanhThu = doanhThuRaw.Select(x => (int)x.Total).ToArray();

                // =========================
                // 3. Thể loại phổ biến
                // =========================
                var theLoaiRaw = await _context.Saches
                    .Include(s => s.ChuDe)
                    .Where(s => s.ChuDe != null)
                    .GroupBy(s => s.ChuDe.TenChuDe)
                    .Select(g => new { Ten = g.Key, SoLuong = g.Count() })
                    .OrderByDescending(g => g.SoLuong)
                    .Take(5)
                    .ToListAsync();

                var theLoai = theLoaiRaw.Select(x => x.SoLuong).ToArray();
                var theLoaiLabels = theLoaiRaw.Select(x => x.Ten).ToArray();

                // =========================
                // 4. Top sách bán chạy
                // =========================
                var topSachRaw = await _context.ChiTietDonHangs
                    .Include(ct => ct.DonHang)
                    .Include(ct => ct.Sach)
                    .Where(ct => ct.DonHang.DaThanhToan && ct.DonHang.NgayTao >= startDate)
                    .GroupBy(ct => ct.Sach.TenSach)
                    .Select(g => new { TenSach = g.Key, SoLuongBan = g.Sum(x => x.SoLuong) })
                    .OrderByDescending(g => g.SoLuongBan)
                    .Take(5)
                    .ToListAsync();

                var topSach = topSachRaw.Select(x => x.SoLuongBan).ToArray();
                var topSachLabels = topSachRaw.Select(x => x.TenSach).ToArray();

                // =========================
                // 5. Top tác giả bán chạy
                // =========================
                var topTacGiaRaw = await _context.ChiTietDonHangs
                    .Include(ct => ct.DonHang)
                    .Include(ct => ct.Sach)
                        .ThenInclude(s => s.TacGia)
                    .Where(ct => ct.DonHang.DaThanhToan &&
                                 ct.DonHang.NgayTao >= startDate &&
                                 ct.Sach.TacGia != null)
                    .GroupBy(ct => ct.Sach.TacGia.TenTG)
                    .Select(g => new { TacGia = g.Key, SoLuongBan = g.Sum(x => x.SoLuong) })
                    .OrderByDescending(g => g.SoLuongBan)
                    .Take(5)
                    .ToListAsync();

                var topTacGia = topTacGiaRaw.Select(x => x.SoLuongBan).ToArray();
                var topTacGiaLabels = topTacGiaRaw.Select(x => x.TacGia).ToArray();

                // =========================
                // 6. Trạng thái đơn hàng
                // =========================
                var trangThaiRaw = await _context.DonHangs
                    .Where(dh => dh.NgayTao >= startDate)
                    .GroupBy(dh => dh.DaThanhToan)
                    .Select(g => new
                    {
                        TrangThai = g.Key ? "Đã thanh toán" : "Chưa thanh toán",
                        SoLuong = g.Count()
                    })
                    .ToListAsync();

                var trangThai = trangThaiRaw.Select(x => x.SoLuong).ToArray();
                var trangThaiLabels = trangThaiRaw.Select(x => x.TrangThai).ToArray();

                // =========================
                // 7. Hình thức thanh toán
                // =========================
                var hinhThucRaw = await _context.DonHangs
                    .Where(dh => dh.NgayTao >= startDate)
                    .GroupBy(dh => dh.HinhThucThanhToan)
                    .Select(g => new { Label = g.Key.ToString(), SoLuong = g.Count() })
                    .ToListAsync();

                var hinhThucThanhToan = hinhThucRaw.Select(x => x.SoLuong).ToArray();
                var hinhThucThanhToanLabels = hinhThucRaw.Select(x => x.Label).ToArray();

                // =========================
                // RETURN JSON
                // =========================
                return Json(new
                {
                    kpi = kpiData,
                    labels = labels.Length == 0 ? new[] { "T2", "T3", "T4", "T5", "T6", "T7", "CN" } : labels,
                    doanhThu = doanhThu.Length == 0 ? new[] { 0, 0, 0, 0, 0, 0, 0 } : doanhThu,

                    theLoai = theLoai.Length == 0 ? new[] { 0, 0, 0 } : theLoai,
                    theLoaiLabels = theLoaiLabels.Length == 0 ? new[] { "Chưa có dữ liệu" } : theLoaiLabels,

                    topSach = topSach.Length == 0 ? new[] { 0, 0, 0, 0, 0 } : topSach,
                    topSachLabels = topSachLabels.Length == 0 ? new[] { "Không có dữ liệu" } : topSachLabels,

                    topTacGia = topTacGia.Length == 0 ? new[] { 0, 0, 0, 0, 0 } : topTacGia,
                    topTacGiaLabels = topTacGiaLabels.Length == 0 ? new[] { "Không có dữ liệu" } : topTacGiaLabels,

                    trangThai = trangThai.Length == 0 ? new[] { 0, 0 } : trangThai,
                    trangThaiLabels = trangThaiLabels.Length == 0 ? new[] { "Đã thanh toán", "Chưa thanh toán" } : trangThaiLabels,

                    hinhThucThanhToan = hinhThucThanhToan.Length == 0 ? new[] { 0, 0 } : hinhThucThanhToan,
                    hinhThucThanhToanLabels = hinhThucThanhToanLabels.Length == 0 ? new[] { "Tiền mặt", "Chuyển khoản" } : hinhThucThanhToanLabels
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Dashboard Error: {ex.Message}");
                return Json(new { success = false, message = ex.Message });
            }
        }

        public IActionResult Index()
        {
            return View();
        }
    }
}
