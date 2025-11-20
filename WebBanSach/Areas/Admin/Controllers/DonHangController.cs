using Media.Models;
using Meida.DataAccess.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace Media.Controllers
{
    [Area("Admin")]
    public class DonHangController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DonHangController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: DonHang
        public async Task<IActionResult> Index()
        {
            return RedirectToAction(nameof(QuanLyDonHang));
        }

        public async Task<IActionResult> QuanLyDonHang(int page = 1, int pageSize = 5, string search = "", string status = "")
        {
            try
            {
                // Base query
                var query = _context.DonHangs
                    .Include(dh => dh.KhachHang)
                    .Include(dh => dh.NhanVien)
                    .Include(dh => dh.VanChuyen)
                    .Include(dh => dh.ChiTietDonHangs)
                    .ThenInclude(ct => ct.Sach)
                    .AsQueryable();

                // Search filter
                if (!string.IsNullOrEmpty(search))
                {
                    query = query.Where(dh =>
                        dh.MaDonHang.ToString().Contains(search) ||
                        dh.KhachHang.HoTen.Contains(search) ||
                        dh.SoDienThoaiNhan.Contains(search) ||
                        dh.TenNguoiNhan.Contains(search));
                }

                // Status filter - Chỉ còn 2 trạng thái thanh toán
                if (!string.IsNullOrEmpty(status))
                {
                    switch (status.ToLower())
                    {
                        case "paid":
                            query = query.Where(dh => dh.DaThanhToan);
                            break;
                        case "pending":
                            query = query.Where(dh => !dh.DaThanhToan);
                            break;
                    }
                }

                // Total counts for stats
                var totalOrders = await query.CountAsync();
                var paidOrders = await query.CountAsync(dh => dh.DaThanhToan);
                var pendingOrders = await query.CountAsync(dh => !dh.DaThanhToan);
                var totalRevenue = await query.Where(dh => dh.DaThanhToan).SumAsync(dh => dh.Total);

                // Pagination
                var donHangs = await query
                    .OrderByDescending(dh => dh.NgayTao)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                // Pass data to view
                ViewBag.CurrentPage = page;
                ViewBag.TotalPages = (int)Math.Ceiling(totalOrders / (double)pageSize);
                ViewBag.TotalOrders = totalOrders;
                ViewBag.PaidOrders = paidOrders;
                ViewBag.PendingOrders = pendingOrders;
                ViewBag.TotalRevenue = totalRevenue;
                ViewBag.Search = search;
                ViewBag.Status = status;
                ViewBag.PageSize = pageSize;

                return View(donHangs);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Lỗi khi tải dữ liệu: " + ex.Message;
                return View(new List<DonHang>());
            }
        }

        // GET: DonHang/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var donHang = await _context.DonHangs
                .Include(dh => dh.KhachHang)
                .Include(dh => dh.NhanVien)
                .Include(dh => dh.VanChuyen)
                .Include(dh => dh.ChiTietDonHangs)
                .ThenInclude(ct => ct.Sach)
                .FirstOrDefaultAsync(m => m.MaDonHang == id);

            if (donHang == null)
            {
                return NotFound();
            }

            return View(donHang);
        }

        // GET: DonHang/Edit/5 - ĐÃ SỬA
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var donHang = await _context.DonHangs
                .Include(dh => dh.ChiTietDonHangs)
                .ThenInclude(ct => ct.Sach)
                .Include(dh => dh.KhachHang)
                .Include(dh => dh.NhanVien)
                .FirstOrDefaultAsync(m => m.MaDonHang == id);

            if (donHang == null)
            {
                return NotFound();
            }

            // Thêm danh sách sách cho dropdown
            ViewBag.Saches = new SelectList(_context.Saches, "MaSach", "TenSach");

            LoadViewData();
            return View(donHang);
        }

        // POST: DonHang/Edit/5 - ĐÃ SỬA HOÀN TOÀN
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, DonHang donHang)
        {
            if (id != donHang.MaDonHang)
            {
                TempData["Error"] = "ID đơn hàng không khớp!";
                return RedirectToAction(nameof(QuanLyDonHang));
            }

            // DEBUG: Kiểm tra lỗi ModelState
            if (!ModelState.IsValid)
            {
                // Lấy tất cả lỗi để debug
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();

                var errorMessage = "Dữ liệu không hợp lệ! Lỗi: " + string.Join("; ", errors);
                TempData["Error"] = errorMessage;

                Console.WriteLine("=== MODEL STATE ERRORS ===");
                foreach (var error in errors)
                {
                    Console.WriteLine(error);
                }

                ViewBag.Saches = new SelectList(_context.Saches, "MaSach", "TenSach");
                LoadViewData();
                return View(donHang);
            }

            try
            {
                // Lấy đơn hàng hiện tại từ database
                var existingDonHang = await _context.DonHangs
                    .Include(dh => dh.ChiTietDonHangs)
                    .FirstOrDefaultAsync(dh => dh.MaDonHang == id);

                if (existingDonHang == null)
                {
                    TempData["Error"] = "Không tìm thấy đơn hàng!";
                    return RedirectToAction(nameof(QuanLyDonHang));
                }

                // Cập nhật thông tin cơ bản - GIỮ NGUYÊN ENUM
                existingDonHang.MaKhachHang = donHang.MaKhachHang;
                existingDonHang.MaNhanVien = donHang.MaNhanVien;
                existingDonHang.TenNguoiNhan = donHang.TenNguoiNhan;
                existingDonHang.SoDienThoaiNhan = donHang.SoDienThoaiNhan;
                existingDonHang.DiaChiChiTiet = donHang.DiaChiChiTiet;
                existingDonHang.PhuongXa = donHang.PhuongXa;
                existingDonHang.QuanHuyen = donHang.QuanHuyen;
                existingDonHang.TinhThanh = donHang.TinhThanh;

                // QUAN TRỌNG: Giữ nguyên enum, không cần chuyển đổi
                existingDonHang.HinhThucThanhToan = donHang.HinhThucThanhToan;

                existingDonHang.DaThanhToan = donHang.DaThanhToan;
                existingDonHang.GhiChu = donHang.GhiChu;
                existingDonHang.NgayCapNhat = DateTime.Now;

                // KHÔNG xử lý chi tiết đơn hàng ở đây nữa - sẽ dùng AJAX
                existingDonHang.Total = existingDonHang.ChiTietDonHangs?.Sum(ct => ct.ThanhTien) ?? 0;

                _context.Update(existingDonHang);
                await _context.SaveChangesAsync();

                TempData["Success"] = "Cập nhật đơn hàng thành công!";
                return RedirectToAction(nameof(QuanLyDonHang));
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!DonHangExists(donHang.MaDonHang))
                {
                    TempData["Error"] = "Đơn hàng không tồn tại!";
                    return RedirectToAction(nameof(QuanLyDonHang));
                }
                else
                {
                    throw;
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Lỗi khi cập nhật đơn hàng: " + ex.Message;
                ViewBag.Saches = new SelectList(_context.Saches, "MaSach", "TenSach");
                LoadViewData();
                return View(donHang);
            }
        }

        // GET: DonHang/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var donHang = await _context.DonHangs
                .Include(dh => dh.KhachHang)
                .Include(dh => dh.NhanVien)
                .Include(dh => dh.ChiTietDonHangs)
                .ThenInclude(ct => ct.Sach)
                .FirstOrDefaultAsync(m => m.MaDonHang == id);

            if (donHang == null)
            {
                return NotFound();
            }

            return View(donHang);
        }

        // POST: DonHang/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var donHang = await _context.DonHangs
                    .Include(dh => dh.ChiTietDonHangs)
                    .Include(dh => dh.VanChuyen)
                    .FirstOrDefaultAsync(dh => dh.MaDonHang == id);

                if (donHang == null)
                {
                    TempData["Error"] = $"Không tìm thấy đơn hàng #{id}";
                    return RedirectToAction(nameof(QuanLyDonHang));
                }

                // Xóa các bản ghi liên quan trước
                if (donHang.ChiTietDonHangs?.Any() == true)
                {
                    _context.ChiTietDonHangs.RemoveRange(donHang.ChiTietDonHangs);
                }

                // Xóa vận chuyển nếu có
                if (donHang.VanChuyen != null)
                {
                    _context.VanChuyens.Remove(donHang.VanChuyen);
                }

                // Xóa đơn hàng
                _context.DonHangs.Remove(donHang);
                await _context.SaveChangesAsync();

                TempData["Success"] = $"Đã xóa đơn hàng #{id} thành công!";
                return RedirectToAction(nameof(QuanLyDonHang));
            }
            catch (DbUpdateException dbEx)
            {
                TempData["Error"] = $"Lỗi database: {dbEx.InnerException?.Message ?? dbEx.Message}";
                return RedirectToAction(nameof(QuanLyDonHang));
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Lỗi: {ex.Message}";
                return RedirectToAction(nameof(QuanLyDonHang));
            }
        }

        // Cập nhật trạng thái thanh toán
        [HttpPost]
        public async Task<IActionResult> UpdatePaymentStatus(int id, bool daThanhToan)
        {
            try
            {
                var donHang = await _context.DonHangs.FindAsync(id);
                if (donHang == null)
                {
                    return Json(new { success = false, message = "Không tìm thấy đơn hàng!" });
                }

                donHang.DaThanhToan = daThanhToan;
                donHang.NgayCapNhat = DateTime.Now;

                _context.Update(donHang);
                await _context.SaveChangesAsync();

                return Json(new { success = true, message = "Cập nhật trạng thái thanh toán thành công!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Lỗi khi cập nhật: " + ex.Message });
            }
        }

        // THÊM MỚI: Action để thêm chi tiết đơn hàng
        [HttpPost]
        public async Task<IActionResult> AddChiTietDonHang(int maDonHang, int maSach, int soLuong, decimal donGia)
        {
            try
            {
                // Kiểm tra tồn tại
                var donHang = await _context.DonHangs.FindAsync(maDonHang);
                var sach = await _context.Saches.FindAsync(maSach);

                if (donHang == null || sach == null)
                {
                    return Json(new { success = false, message = "Không tìm thấy đơn hàng hoặc sách!" });
                }

                // Kiểm tra số lượng tồn kho
                if (sach.SoLuong < soLuong)
                {
                    return Json(new { success = false, message = $"Số lượng tồn kho không đủ. Chỉ còn {sach.SoLuong} sách!" });
                }

                var chiTiet = new ChiTietDonHang
                {
                    MaDonHang = maDonHang,
                    MaSach = maSach,
                    SoLuong = soLuong,
                    DonGia = donGia,
                    ThanhTien = soLuong * donGia
                };

                _context.ChiTietDonHangs.Add(chiTiet);
                await _context.SaveChangesAsync();

                // Cập nhật tổng tiền đơn hàng
                await UpdateTotalDonHang(maDonHang);

                return Json(new { success = true, message = "Thêm sách thành công!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Lỗi: " + ex.Message });
            }
        }

        // THÊM MỚI: Action để xóa chi tiết đơn hàng
        [HttpPost]
        public async Task<IActionResult> RemoveChiTietDonHang(int id)
        {
            try
            {
                var chiTiet = await _context.ChiTietDonHangs
                    .Include(ct => ct.DonHang)
                    .FirstOrDefaultAsync(ct => ct.MaDonHang == id);

                if (chiTiet != null)
                {
                    _context.ChiTietDonHangs.Remove(chiTiet);
                    await _context.SaveChangesAsync();

                    // Cập nhật tổng tiền đơn hàng
                    await UpdateTotalDonHang(chiTiet.MaDonHang);

                    return Json(new { success = true, message = "Xóa sách thành công!" });
                }
                return Json(new { success = false, message = "Không tìm thấy chi tiết!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Lỗi: " + ex.Message });
            }
        }

        // THÊM MỚI: Lấy giá sách
        [HttpGet]
        public async Task<decimal> GetBookPrice(int maSach)
        {
            var sach = await _context.Saches.FindAsync(maSach);
            return sach?.GiaBan ?? 0;
        }

        private bool DonHangExists(int id)
        {
            return _context.DonHangs.Any(e => e.MaDonHang == id);
        }

        private void LoadViewData()
        {
            try
            {
                ViewBag.KhachHangs = new SelectList(_context.KhachHangs, "MaKhachHang", "HoTen");
                ViewBag.NhanViens = new SelectList(_context.NhanViens, "MaNhanVien", "HoTen");

                // SỬA LẠI: Sử dụng giá trị string của enum thay vì số
                ViewBag.HinhThucThanhToan = new SelectList(
                    Enum.GetValues(typeof(HinhThucThanhToan))
                        .Cast<HinhThucThanhToan>()
                        .Select(e => new {
                            Value = e.ToString(), // SỬA: e.ToString() thay vì (int)e
                            Text = GetEnumDescription(e)
                        }),
                    "Value",
                    "Text"
                );
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi khi load ViewData: {ex.Message}");
            }
        }

        // THÊM MỚI: Hàm lấy description từ enum
        private string GetEnumDescription(Enum value)
        {
            var field = value.GetType().GetField(value.ToString());
            var attribute = (DescriptionAttribute)Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute));
            return attribute == null ? value.ToString() : attribute.Description;
        }

        // THÊM MỚI: Cập nhật tổng tiền đơn hàng
        private async Task UpdateTotalDonHang(int maDonHang)
        {
            var donHang = await _context.DonHangs
                .Include(dh => dh.ChiTietDonHangs)
                .FirstOrDefaultAsync(dh => dh.MaDonHang == maDonHang);

            if (donHang != null)
            {
                donHang.Total = donHang.ChiTietDonHangs?.Sum(ct => ct.ThanhTien) ?? 0;
                donHang.NgayCapNhat = DateTime.Now;
                await _context.SaveChangesAsync();
            }
        }
    }
}