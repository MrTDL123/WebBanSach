using Media.Models;
using Meida.DataAccess.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
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

        // GET: DonHang
        public async Task<IActionResult> QuanLyDonHang()
        {
            var donHangs = await _context.DonHangs
                .Include(dh => dh.KhachHang)
                .Include(dh => dh.NhanVien)
                .Include(dh => dh.VanChuyen)
                .Include(dh => dh.ChiTietDonHangs)
                .ThenInclude(ct => ct.Sach)
                .OrderByDescending(dh => dh.NgayTao)
                .ToListAsync();

            return View(donHangs);
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

        // GET: DonHang/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var donHang = await _context.DonHangs
                .Include(dh => dh.ChiTietDonHangs)
                .ThenInclude(ct => ct.Sach)
                .FirstOrDefaultAsync(m => m.MaDonHang == id);

            if (donHang == null)
            {
                return NotFound();
            }

            LoadViewData();
            return View(donHang);
        }

        // POST: DonHang/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, DonHang donHang)
        {
            if (id != donHang.MaDonHang)
            {
                return NotFound();
            }

            try
            {
                // Lấy chi tiết đơn hàng từ form
                var form = await Request.ReadFormAsync();
                var chiTietDonHangs = new List<ChiTietDonHang>();

                var maSachValues = form["chiTietDonHangs[0].MaSach"];
                var soLuongValues = form["chiTietDonHangs[0].SoLuong"];
                var donGiaValues = form["chiTietDonHangs[0].DonGia"];
                var thanhTienValues = form["chiTietDonHangs[0].ThanhTien"];

                if (!string.IsNullOrEmpty(maSachValues) && maSachValues != "0")
                {
                    var chiTiet = new ChiTietDonHang
                    {
                        MaSach = int.Parse(maSachValues),
                        SoLuong = int.Parse(soLuongValues),
                        DonGia = decimal.Parse(donGiaValues),
                        ThanhTien = decimal.Parse(thanhTienValues)
                    };
                    chiTietDonHangs.Add(chiTiet);
                }

                // Cập nhật thông tin đơn hàng
                donHang.NgayCapNhat = DateTime.Now;
                donHang.Total = chiTietDonHangs.Sum(ct => ct.ThanhTien);

                _context.Update(donHang);

                // Xóa chi tiết cũ và thêm chi tiết mới
                var existingChiTiet = _context.ChiTietDonHangs.Where(ct => ct.MaDonHang == id);
                _context.ChiTietDonHangs.RemoveRange(existingChiTiet);

                foreach (var chiTiet in chiTietDonHangs)
                {
                    chiTiet.MaDonHang = id;
                    _context.ChiTietDonHangs.Add(chiTiet);
                }

                await _context.SaveChangesAsync();

                TempData["Success"] = "Cập nhật đơn hàng thành công!";
                return RedirectToAction(nameof(QuanLyDonHang));
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!DonHangExists(donHang.MaDonHang))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Lỗi khi cập nhật đơn hàng: " + ex.Message);
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
                var donHang = await _context.DonHangs.FindAsync(id);
                if (donHang != null)
                {
                    // Xóa chi tiết đơn hàng trước
                    var chiTietDonHangs = _context.ChiTietDonHangs.Where(ct => ct.MaDonHang == id);
                    _context.ChiTietDonHangs.RemoveRange(chiTietDonHangs);

                    _context.DonHangs.Remove(donHang);
                    await _context.SaveChangesAsync();

                    TempData["Success"] = "Xóa đơn hàng thành công!";
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Lỗi khi xóa đơn hàng: " + ex.Message;
            }

            return RedirectToAction(nameof(QuanLyDonHang));
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
                    return NotFound();
                }

                donHang.DaThanhToan = daThanhToan;
                donHang.NgayCapNhat = DateTime.Now;

                _context.Update(donHang);
                await _context.SaveChangesAsync();

                TempData["Success"] = "Cập nhật trạng thái thanh toán thành công!";
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Lỗi khi cập nhật: " + ex.Message;
            }

            return RedirectToAction(nameof(QuanLyDonHang));
        }

        // Lấy danh sách sách cho dropdown
        [HttpGet]
        public async Task<JsonResult> GetSachs()
        {
            try
            {
                var sachs = await _context.Saches
                    .Select(s => new {
                        maSach = s.MaSach,
                        tenSach = s.TenSach,
                        donGia = s.GiaBan
                    })
                    .ToListAsync();

                return Json(sachs);
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message });
            }
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
                ViewBag.VanChuyens = new SelectList(_context.VanChuyens, "MaVanChuyen", "DonViVanChuyen");

                // Enum cho hình thức thanh toán
                ViewBag.HinhThucThanhToan = new List<SelectListItem>
                {
                    new SelectListItem { Value = "0", Text = "Thanh toán trực tiếp" },
                    new SelectListItem { Value = "1", Text = "Chuyển khoản" }
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi khi load ViewData: {ex.Message}");
            }
        }
    }
}