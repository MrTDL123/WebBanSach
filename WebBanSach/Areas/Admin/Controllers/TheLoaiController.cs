using Media.Models;
using Meida.DataAccess.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ProjectCuoiKi.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class TheLoaiController : Controller
    {
        private readonly ApplicationDbContext _db;

        public TheLoaiController(ApplicationDbContext db)
        {
            _db = db;
        }

        // ========== QUẢN LÝ THỂ LOẠI ==========
        public IActionResult QuanLyTheLoai()
        {
            var theLoais = _db.ChuDes.ToList();
            return View(theLoais);
        }

        // GET: Thêm thể loại
        public IActionResult ThemTheLoai()
        {
            ViewBag.ParentCategories = _db.ChuDes.Where(c => c.ParentId == null).ToList();
            return View();
        }

        // POST: Thêm thể loại
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ThemTheLoai(ChuDe model)
        {
            ViewBag.ParentCategories = _db.ChuDes.Where(c => c.ParentId == null).ToList();

            if (ModelState.IsValid)
            {
                // Kiểm tra tên thể loại đã tồn tại chưa
                var existingCategory = await _db.ChuDes
                    .AsNoTracking()
                    .FirstOrDefaultAsync(c => c.TenChuDe.ToLower() == model.TenChuDe.ToLower());

                if (existingCategory != null)
                {
                    ModelState.AddModelError("TenChuDe", "Tên thể loại đã tồn tại trong hệ thống");
                    return View(model);
                }

                _db.ChuDes.Add(model);
                await _db.SaveChangesAsync();

                TempData["Success"] = "Thêm thể loại thành công!";
                return RedirectToAction("QuanLyTheLoai");
            }
            return View(model);
        }

        // GET: Sửa thể loại
        public async Task<IActionResult> SuaTheLoai(int id)
        {
            var theLoai = await _db.ChuDes
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.MaChuDe == id);

            if (theLoai == null)
            {
                return NotFound();
            }

            ViewBag.ParentCategories = _db.ChuDes
                .AsNoTracking()
                .Where(c => c.ParentId == null)
                .ToList();

            return View(theLoai);
        }

        // POST: Sửa thể loại - ĐÃ SỬA LỖI TRACKING
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SuaTheLoai(int id, ChuDe model)
        {
            if (id != model.MaChuDe)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Kiểm tra tên thể loại đã tồn tại chưa (trừ chính nó)
                    var existingCategory = await _db.ChuDes
                        .AsNoTracking()
                        .FirstOrDefaultAsync(c => c.TenChuDe.ToLower() == model.TenChuDe.ToLower() && c.MaChuDe != id);

                    if (existingCategory != null)
                    {
                        ModelState.AddModelError("TenChuDe", "Tên thể loại đã tồn tại trong hệ thống");
                        ViewBag.ParentCategories = _db.ChuDes
                            .AsNoTracking()
                            .Where(c => c.ParentId == null)
                            .ToList();
                        return View(model);
                    }

                    // CÁCH 1: Attach và set trạng thái Modified
                    var existingEntity = await _db.ChuDes.FindAsync(id);
                    if (existingEntity != null)
                    {
                        // Copy values từ model vào existingEntity
                        existingEntity.TenChuDe = model.TenChuDe;
                        existingEntity.Slug = model.Slug;
                        existingEntity.FullPath = model.FullPath;
                        existingEntity.ParentId = model.ParentId;

                        _db.ChuDes.Update(existingEntity);
                        await _db.SaveChangesAsync();
                    }

                    TempData["Success"] = "Cập nhật thể loại thành công!";
                    return RedirectToAction("QuanLyTheLoai");
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ChuDeExists(model.MaChuDe))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }

            ViewBag.ParentCategories = _db.ChuDes
                .AsNoTracking()
                .Where(c => c.ParentId == null)
                .ToList();
            return View(model);
        }

        // GET: Xóa thể loại
        public async Task<IActionResult> XoaTheLoai(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var theLoai = await _db.ChuDes
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.MaChuDe == id);

            if (theLoai == null)
            {
                return NotFound();
            }

            // Kiểm tra xem thể loại có sách nào không
            var coSach = await _db.Saches
                .AsNoTracking()
                .AnyAsync(s => s.MaChuDe == id);
            ViewBag.CoSach = coSach;

            return View(theLoai);
        }

        // POST: Xóa thể loại
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> XoaTheLoaiConfirmed(int id)
        {
            try
            {
                var theLoai = await _db.ChuDes.FindAsync(id);
                if (theLoai == null)
                {
                    return NotFound();
                }

                // Kiểm tra xem thể loại có sách nào không
                var coSach = await _db.Saches
                    .AsNoTracking()
                    .AnyAsync(s => s.MaChuDe == id);
                if (coSach)
                {
                    TempData["Error"] = "Không thể xóa thể loại vì có sách liên quan!";
                    return RedirectToAction(nameof(QuanLyTheLoai));
                }

                _db.ChuDes.Remove(theLoai);
                await _db.SaveChangesAsync();

                TempData["Success"] = "Xóa thể loại thành công!";
                return RedirectToAction(nameof(QuanLyTheLoai));
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Lỗi khi xóa thể loại: " + ex.Message;
                return RedirectToAction(nameof(QuanLyTheLoai));
            }
        }

        private bool ChuDeExists(int id)
        {
            return _db.ChuDes
                .AsNoTracking()
                .Any(e => e.MaChuDe == id);
        }
    }
}