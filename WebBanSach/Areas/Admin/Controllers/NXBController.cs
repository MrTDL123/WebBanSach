using Media.Areas.Admin.Controllers;
using Media.Models;
using Meida.DataAccess.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ProjectCuoiKi.Areas.Admin.Controllers 
{
    public class NXBController : AdminController
    {
        private readonly ApplicationDbContext _db;

        public NXBController(ApplicationDbContext db)
        {
            _db = db;
        }

        public IActionResult QuanLyNXB()
        {
            var nxbList = _db.NhaXuatBans.ToList();
            ViewBag.TongSach = _db.Saches.Count();
            return View(nxbList);
        }


        public IActionResult ThemNXB()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ThemNXB(NhaXuatBan model)
        {
            if (ModelState.IsValid)
            {
                // Kiểm tra tên NXB đã tồn tại chưa
                var existingNXB = await _db.NhaXuatBans
                    .FirstOrDefaultAsync(n => n.TenNXB.ToLower() == model.TenNXB.ToLower());

                if (existingNXB != null)
                {
                    ModelState.AddModelError("TenNXB", "Tên nhà xuất bản đã tồn tại trong hệ thống");
                    return View(model);
                }

                _db.NhaXuatBans.Add(model);
                await _db.SaveChangesAsync();

                TempData["Success"] = "Thêm nhà xuất bản thành công!";
                return RedirectToAction("QuanLyNXB");
            }
            return View(model);
        }

        public async Task<IActionResult> SuaNXB(int id)
        {
            var nxb = await _db.NhaXuatBans.FindAsync(id);
            if (nxb == null) return NotFound();
            return View(nxb);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SuaNXB(int id, NhaXuatBan model)
        {
            if (id != model.MaNhaXuatBan)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var existingNXB = await _db.NhaXuatBans
                        .FirstOrDefaultAsync(n => n.TenNXB.ToLower() == model.TenNXB.ToLower() && n.MaNhaXuatBan != id);

                    if (existingNXB != null)
                    {
                        ModelState.AddModelError("TenNXB", "Tên nhà xuất bản đã tồn tại trong hệ thống");
                        return View(model);
                    }

                    _db.NhaXuatBans.Update(model);
                    await _db.SaveChangesAsync();

                    TempData["Success"] = "Cập nhật nhà xuất bản thành công!";
                    return RedirectToAction("QuanLyNXB");
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!NhaXuatBanExists(model.MaNhaXuatBan))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }
            return View(model);
        }

        public async Task<IActionResult> XoaNXB(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var nxb = await _db.NhaXuatBans.FindAsync(id);
            if (nxb == null) return NotFound();

            var coSach = await _db.Saches.AnyAsync(s => s.MaNhaXuatBan == id);
            var soSach = await _db.Saches.CountAsync(s => s.MaNhaXuatBan == id);

            ViewBag.CoSach = coSach;
            ViewBag.SoSach = soSach;

            return View(nxb);
        }

        [HttpPost, ActionName("XoaNXB")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> XoaNXBConfirmed(int id)
        {
            try
            {
                var nxb = await _db.NhaXuatBans.FindAsync(id);
                if (nxb == null) return NotFound();

                var coSach = await _db.Saches.AnyAsync(s => s.MaNhaXuatBan == id);
                var soSach = await _db.Saches.CountAsync(s => s.MaNhaXuatBan == id);

                if (coSach)
                {
                    TempData["Error"] = $"Không thể xóa nhà xuất bản '{nxb.TenNXB}' vì có {soSach} sách thuộc nhà xuất bản này!";
                    return RedirectToAction("QuanLyNXB");
                }

                _db.NhaXuatBans.Remove(nxb);
                await _db.SaveChangesAsync();

                TempData["Success"] = "Xóa nhà xuất bản thành công!";
                return RedirectToAction("QuanLyNXB");
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Lỗi khi xóa nhà xuất bản: " + ex.Message;
                return RedirectToAction("QuanLyNXB");
            }
        }

        private bool NhaXuatBanExists(int id)
        {
            return _db.NhaXuatBans.Any(e => e.MaNhaXuatBan == id);
        }
    }
}