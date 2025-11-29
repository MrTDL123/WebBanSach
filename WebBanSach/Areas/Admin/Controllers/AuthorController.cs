using Media.Areas.Admin.Controllers;
using Media.Models;
using Meida.DataAccess.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ProjectCuoiKi.Areas.Admin.Controllers
{
    public class AuthorController : AdminController
    {
        private readonly ApplicationDbContext _db;

        public AuthorController(ApplicationDbContext db)
        {
            _db = db;
        }

        // ========== QUẢN LÝ TÁC GIẢ ==========
        // GET: Danh sách tác giả với tìm kiếm
        public async Task<IActionResult> QuanLyTacGia(string searchString)
        {
            var tacGias = from tg in _db.TacGias
                          select tg;

            if (!string.IsNullOrEmpty(searchString))
            {
                tacGias = tacGias.Where(tg => tg.TenTG.Contains(searchString) ||
                                             (tg.QuocTich != null && tg.QuocTich.Contains(searchString)) ||
                                             (tg.TieuSu != null && tg.TieuSu.Contains(searchString)));
            }

            tacGias = tacGias.OrderBy(tg => tg.TenTG);

            // ✅ SỬA: Tính tổng số sách chính xác
            var tongSach = await _db.Saches.CountAsync();
            ViewBag.TongSach = tongSach;
            ViewData["CurrentFilter"] = searchString;

            return View(await tacGias.AsNoTracking().ToListAsync());
        }

        // GET: Thêm tác giả
        public IActionResult ThemTacGia()
        {
            // ✅ THÊM DÒNG NÀY: Khởi tạo model mới
            var model = new TacGia();
            return View(model);
        }

        // ✅ SỬA: Xóa dòng trùng lặp
        // POST: Thêm tác giả
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ThemTacGia(TacGia tacGia)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    // Kiểm tra tên tác giả đã tồn tại chưa
                    var existingTacGia = await _db.TacGias
                        .FirstOrDefaultAsync(tg => tg.TenTG.ToLower() == tacGia.TenTG.ToLower());

                    if (existingTacGia != null)
                    {
                        ModelState.AddModelError("TenTG", "Tên tác giả đã tồn tại trong hệ thống");
                        TempData["Error"] = "Tên tác giả đã tồn tại trong hệ thống";
                        return View(tacGia);
                    }

                    // Đảm bảo các trường nullable được xử lý đúng
                    tacGia.TieuSu ??= string.Empty;
                    tacGia.QuocTich ??= string.Empty;

                    // Thêm tác giả mới
                    _db.TacGias.Add(tacGia);
                    await _db.SaveChangesAsync();

                    TempData["Success"] = "Thêm tác giả thành công!";
                    return RedirectToAction(nameof(QuanLyTacGia));
                }

                // ✅ THÊM: Trả về view với model hiện tại nếu có lỗi validation
                return View(tacGia);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Lỗi khi thêm tác giả: " + ex.Message);
                TempData["Error"] = "Lỗi khi thêm tác giả: " + ex.Message;

                // ✅ QUAN TRỌNG: Trả về view với model hiện tại
                return View(tacGia);
            }
        }

        // GET: Sửa tác giả
        // GET: Sửa tác giả
        public async Task<IActionResult> SuaTacGia(int id)
        {
            var tacGia = await _db.TacGias.FindAsync(id);
            if (tacGia == null)
            {
                return NotFound();
            }
            return View(tacGia);
        }

        // POST: Sửa tác giả
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SuaTacGia(int id, TacGia tacGia)
        {
            if (id != tacGia.MaTacGia)
            {
                TempData["Error"] = "ID không khớp!";
                return View(tacGia);
            }

            if (!ModelState.IsValid)
            {
                return View(tacGia);
            }

            try
            {
                // Kiểm tra tên tác giả đã tồn tại chưa (trừ chính nó)
                var existingTacGia = await _db.TacGias
                    .FirstOrDefaultAsync(tg => tg.TenTG.ToLower() == tacGia.TenTG.ToLower() && tg.MaTacGia != id);

                if (existingTacGia != null)
                {
                    ModelState.AddModelError("TenTG", "Tên tác giả đã tồn tại trong hệ thống");
                    TempData["Error"] = "Tên tác giả đã tồn tại trong hệ thống";
                    return View(tacGia);
                }

                // Lấy tác giả hiện tại từ database
                var existing = await _db.TacGias.FindAsync(id);
                if (existing == null)
                {
                    return NotFound();
                }

                // Cập nhật thông tin
                existing.TenTG = tacGia.TenTG.Trim();
                existing.QuocTich = tacGia.QuocTich?.Trim() ?? string.Empty;
                existing.TieuSu = tacGia.TieuSu?.Trim() ?? string.Empty;

                _db.TacGias.Update(existing);
                await _db.SaveChangesAsync();

                TempData["Success"] = "Cập nhật tác giả thành công!";
                return RedirectToAction(nameof(QuanLyTacGia));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Lỗi khi cập nhật tác giả: " + ex.Message);
                TempData["Error"] = "Lỗi khi cập nhật tác giả: " + ex.Message;
                return View(tacGia);
            }
        }
        // GET: Xóa tác giả
        public async Task<IActionResult> XoaTacGia(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tacGia = await _db.TacGias
                .FirstOrDefaultAsync(m => m.MaTacGia == id);

            if (tacGia == null)
            {
                return NotFound();
            }

            // Kiểm tra xem tác giả có sách nào không
            var coSach = await _db.Saches.AnyAsync(s => s.MaTacGia == id);
            ViewBag.CoSach = coSach;

            return View(tacGia);
        }

        // POST: Xóa tác giả
        [HttpPost, ActionName("XoaTacGia")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> XoaTacGiaConfirmed(int id)
        {
            try
            {
                var tacGia = await _db.TacGias.FindAsync(id);
                if (tacGia == null)
                {
                    return NotFound();
                }

                // Kiểm tra xem tác giả có sách nào không
                var coSach = await _db.Saches.AnyAsync(s => s.MaTacGia == id);
                if (coSach)
                {
                    TempData["Error"] = "Không thể xóa tác giả vì có sách liên quan!";
                    return RedirectToAction(nameof(QuanLyTacGia));
                }

                _db.TacGias.Remove(tacGia);
                await _db.SaveChangesAsync();

                TempData["Success"] = "Xóa tác giả thành công!";
                return RedirectToAction(nameof(QuanLyTacGia));
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Lỗi khi xóa tác giả: " + ex.Message;
                return RedirectToAction(nameof(QuanLyTacGia));
            }
        }

        // GET: Chi tiết tác giả
        public async Task<IActionResult> ChiTietTacGia(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tacGia = await _db.TacGias
                .FirstOrDefaultAsync(m => m.MaTacGia == id);

            if (tacGia == null)
            {
                return NotFound();
            }

            // Lấy danh sách sách của tác giả
            var saches = await _db.Saches
                .Where(s => s.MaTacGia == id)
                .Include(s => s.NhaXuatBan)
                .ToListAsync();

            ViewBag.Saches = saches;
            ViewBag.SoLuongSach = saches.Count;

            return View(tacGia);
        }

        // API: Lấy danh sách tác giả cho dropdown
        [HttpGet]
        public async Task<JsonResult> GetTacGias()
        {
            try
            {
                var tacGias = await _db.TacGias
                    .Select(tg => new {
                        maTacGia = tg.MaTacGia,
                        tenTacGia = tg.TenTG,
                        quocTich = tg.QuocTich
                    })
                    .OrderBy(tg => tg.tenTacGia)
                    .ToListAsync();

                return Json(tacGias);
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message });
            }
        }

        // API: Tìm kiếm tác giả
        [HttpGet]
        public async Task<JsonResult> SearchTacGias(string keyword)
        {
            try
            {
                var tacGias = await _db.TacGias
                    .Where(tg => tg.TenTG.Contains(keyword) || (tg.QuocTich != null && tg.QuocTich.Contains(keyword)))
                    .Select(tg => new {
                        maTacGia = tg.MaTacGia,
                        tenTacGia = tg.TenTG,
                        quocTich = tg.QuocTich
                    })
                    .ToListAsync();

                return Json(tacGias);
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message });
            }
        }

        private bool TacGiaExists(int id)
        {
            return _db.TacGias.Any(e => e.MaTacGia == id);
        }
    }
}