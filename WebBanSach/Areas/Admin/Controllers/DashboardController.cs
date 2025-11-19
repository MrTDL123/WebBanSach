using Media.Models;
using Meida.DataAccess.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
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
                var startDate = DateTime.Now.AddDays(-days);

                // 1. KPI DATA - SỐ LIỆU TỔNG QUAN
                var kpiData = new
                {
                    sach = await _context.Saches.CountAsync(),
                    user = await _context.TaiKhoans.CountAsync(),
                    revenue = await _context.DonHangs
                        .Where(dh => dh.NgayTao >= startDate && dh.DaThanhToan) // Sửa theo model thực tế
                        .SumAsync(dh => dh.Total),
                    order = await _context.DonHangs
                        .Where(dh => dh.NgayTao >= startDate)
                        .CountAsync()
                };

                // 2. DOANH THU THEO NGÀY (7 ngày gần nhất)
                var doanhThuData = await _context.DonHangs
                    .Where(dh => dh.NgayTao >= DateTime.Now.AddDays(-7) && dh.DaThanhToan)
                    .GroupBy(dh => dh.NgayTao.Date)
                    .Select(g => new { Ngay = g.Key, DoanhThu = g.Sum(dh => dh.Total) })
                    .OrderBy(x => x.Ngay)
                    .ToListAsync();

                var labels = doanhThuData.Select(x => x.Ngay.ToString("dd/MM")).ToArray();
                var doanhThu = doanhThuData.Select(x => (int)x.DoanhThu).ToArray();

                // 3. THỂ LOẠI PHỔ BIẾN (dùng ChuDe)
                var theLoaiData = await _context.Saches
                    .Include(s => s.ChuDe)
                    .Where(s => s.ChuDe != null)
                    .GroupBy(s => s.ChuDe.TenChuDe)
                    .Select(g => new { TheLoai = g.Key, SoLuong = g.Count() })
                    .OrderByDescending(x => x.SoLuong)
                    .Take(5)
                    .ToListAsync();

                var theLoai = theLoaiData.Select(x => x.SoLuong).ToArray();
                var theLoaiLabels = theLoaiData.Select(x => x.TheLoai).ToArray();

                // 4. TOP SÁCH BÁN CHẠY (dùng ChiTietDonHang)
                var topSachData = await _context.ChiTietDonHangs
                    .Include(ct => ct.Sach)
                    .Include(ct => ct.DonHang)
                    .Where(ct => ct.DonHang.NgayTao >= startDate && ct.DonHang.DaThanhToan)
                    .GroupBy(ct => new { ct.Sach.MaSach, ct.Sach.TenSach })
                    .Select(g => new { TenSach = g.Key.TenSach, SoLuongBan = g.Sum(ct => ct.SoLuong) })
                    .OrderByDescending(x => x.SoLuongBan)
                    .Take(5)
                    .ToListAsync();

                var topSach = topSachData.Select(x => x.SoLuongBan).ToArray();
                var topSachLabels = topSachData.Select(x => x.TenSach).ToArray();

                // 5. TOP TÁC GIẢ BÁN CHẠY
                var topTacGiaData = await _context.ChiTietDonHangs
                    .Include(ct => ct.Sach)
                    .ThenInclude(s => s.TacGia)
                    .Include(ct => ct.DonHang)
                    .Where(ct => ct.DonHang.NgayTao >= startDate && ct.DonHang.DaThanhToan && ct.Sach.TacGia != null)
                    .GroupBy(ct => new { ct.Sach.TacGia.MaTacGia, ct.Sach.TacGia.TenTG })
                    .Select(g => new { TacGia = g.Key.TenTG, SoLuongBan = g.Sum(ct => ct.SoLuong) })
                    .OrderByDescending(x => x.SoLuongBan)
                    .Take(5)
                    .ToListAsync();

                var topTacGia = topTacGiaData.Select(x => x.SoLuongBan).ToArray();
                var topTacGiaLabels = topTacGiaData.Select(x => x.TacGia).ToArray();

                // 6. TRẠNG THÁI ĐƠN HÀNG
                var trangThaiData = await _context.DonHangs
                    .Where(dh => dh.NgayTao >= startDate)
                    .GroupBy(dh => dh.DaThanhToan)
                    .Select(g => new { TrangThai = g.Key ? "Đã thanh toán" : "Chưa thanh toán", SoLuong = g.Count() })
                    .ToListAsync();

                var trangThai = trangThaiData.Select(x => x.SoLuong).ToArray();
                var trangThaiLabels = trangThaiData.Select(x => x.TrangThai).ToArray();

                // 7. HÌNH THỨC THANH TOÁN
                var hinhThucThanhToanData = await _context.DonHangs
                    .Where(dh => dh.NgayTao >= startDate)
                    .GroupBy(dh => dh.HinhThucThanhToan)
                    .Select(g => new { HinhThuc = g.Key.ToString(), SoLuong = g.Count() })
                    .ToListAsync();

                var hinhThucThanhToan = hinhThucThanhToanData.Select(x => x.SoLuong).ToArray();
                var hinhThucThanhToanLabels = hinhThucThanhToanData.Select(x => x.HinhThuc).ToArray();

                return Json(new
                {
                    kpi = kpiData,
                    labels = labels.Length > 0 ? labels : new[] { "T2", "T3", "T4", "T5", "T6", "T7", "CN" },
                    doanhThu = doanhThu.Length > 0 ? doanhThu : new[] { 0, 0, 0, 0, 0, 0, 0 },
                    theLoai = theLoai.Length > 0 ? theLoai : new[] { 1, 1, 1 },
                    theLoaiLabels = theLoaiLabels.Length > 0 ? theLoaiLabels : new[] { "Chưa có dữ liệu" },
                    topSach = topSach.Length > 0 ? topSach : new[] { 10, 8, 6, 4, 2 },
                    topSachLabels = topSachLabels.Length > 0 ? topSachLabels : new[] { "Chưa có", "dữ liệu", "sách", "bán", "chạy" },
                    topTacGia = topTacGia.Length > 0 ? topTacGia : new[] { 15, 12, 10, 8, 5 },
                    topTacGiaLabels = topTacGiaLabels.Length > 0 ? topTacGiaLabels : new[] { "Chưa có", "dữ liệu", "tác giả", "bán", "chạy" },
                    trangThai = trangThai.Length > 0 ? trangThai : new[] { 1, 1 },
                    trangThaiLabels = trangThaiLabels.Length > 0 ? trangThaiLabels : new[] { "Đã thanh toán", "Chưa thanh toán" },
                    hinhThucThanhToan = hinhThucThanhToan.Length > 0 ? hinhThucThanhToan : new[] { 1, 1 },
                    hinhThucThanhToanLabels = hinhThucThanhToanLabels.Length > 0 ? hinhThucThanhToanLabels : new[] { "Tiền mặt", "Chuyển khoản" }
                });
            }
            catch (Exception ex)
            {
                // Log lỗi và trả về data mẫu
                Console.WriteLine($"Lỗi: {ex.Message}");

                return Json(new
                {
                    kpi = new
                    {
                        sach = await _context.Saches.CountAsync(),
                        user = await _context.TaiKhoans.CountAsync(),
                        revenue = 0,
                        order = 0
                    },
                    labels = new[] { "T2", "T3", "T4", "T5", "T6", "T7", "CN" },
                    doanhThu = new[] { 1200000, 1900000, 1500000, 2500000, 2200000, 3000000, 2800000 },
                    theLoai = new[] { 15, 12, 8, 5, 3 },
                    theLoaiLabels = new[] { "Trinh thám", "Lãng mạn", "Khoa học", "Lịch sử", "Khác" },
                    topSach = new[] { 45, 32, 28, 25, 18 },
                    topSachLabels = new[] { "Sách A", "Sách B", "Sách C", "Sách D", "Sách E" },
                    topTacGia = new[] { 25, 20, 18, 15, 12 },
                    topTacGiaLabels = new[] { "Tác giả A", "Tác giả B", "Tác giả C", "Tác giả D", "Tác giả E" },
                    trangThai = new[] { 35, 10 },
                    trangThaiLabels = new[] { "Đã thanh toán", "Chưa thanh toán" },
                    hinhThucThanhToan = new[] { 30, 15 },
                    hinhThucThanhToanLabels = new[] { "Tiền mặt", "Chuyển khoản" }
                });
            }
        }

        public IActionResult Index()
        {
            return View();
        }
    }
}