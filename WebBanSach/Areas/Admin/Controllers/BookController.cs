using Media.Models;
using Meida.DataAccess.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace ProjectCuoiKi.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class BookController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public BookController(ApplicationDbContext db, IWebHostEnvironment webHostEnvironment)
        {
            _db = db;
            _webHostEnvironment = webHostEnvironment;
        }

        // 📚 Danh sách sách
        public IActionResult QuanLySach()
        {
            var dsSach = _db.Saches
                .Include(s => s.ChuDe)      // Load dữ liệu Chủ đề
                .Include(s => s.TacGia)     // Load dữ liệu Tác giả  
                .Include(s => s.NhaXuatBan) // Load dữ liệu Nhà xuất bản
                .ToList();
            return View(dsSach);
        }

        // ➕ GET: Thêm sách
        [HttpGet]
        public IActionResult ThemSach()
        {
            LoadDropdowns();
            return View(new Sach());
        }

        // ➕ POST: Thêm sách
        [HttpPost]
        public IActionResult ThemSach(Sach model, IFormFile fileAnhBia)
        {
            if (!ModelState.IsValid)
            {
                LoadDropdowns();
                return View(model);
            }

            if (fileAnhBia != null && fileAnhBia.Length > 0)
            {
                string uploadDir = Path.Combine(_webHostEnvironment.WebRootPath, "uploads");
                if (!Directory.Exists(uploadDir)) Directory.CreateDirectory(uploadDir);

                string fileName = Guid.NewGuid().ToString() + Path.GetExtension(fileAnhBia.FileName);
                string path = Path.Combine(uploadDir, fileName);

                using var stream = new FileStream(path, FileMode.Create);
                fileAnhBia.CopyTo(stream);

                model.AnhBiaChinh = fileName;
            }

            model.NgayCapNhat = DateTime.Now;
            _db.Saches.Add(model);
            _db.SaveChanges();

            TempData["Success"] = "Thêm sách thành công!";
            return RedirectToAction("QuanLySach");
        }

        private void LoadDropdowns()
        {
            // Đảm bảo không có null và filter ra chỉ những item có giá trị
            ViewBag.ChuDes = _db.ChuDes?.Where(c => c != null && !string.IsNullOrEmpty(c.TenChuDe)).ToList() ?? new List<ChuDe>();
            ViewBag.TacGias = _db.TacGias?.Where(t => t != null && !string.IsNullOrEmpty(t.TenTG)).ToList() ?? new List<TacGia>();
            ViewBag.NhaXuatBans = _db.NhaXuatBans?.Where(n => n != null && !string.IsNullOrEmpty(n.TenNXB)).ToList() ?? new List<NhaXuatBan>();
        }

        [HttpGet]
        public IActionResult SuaSach(int id)
        {
            var sach = _db.Saches.FirstOrDefault(x => x.MaSach == id);
            if (sach == null) return NotFound();

            LoadDropdowns();
            return View(sach);
        }

        [HttpPost]
        public IActionResult SuaSach(Sach model, IFormFile fileAnhBia)
        {
            // Nếu dữ liệu không hợp lệ, load lại dropdown trước khi trả view
            if (!ModelState.IsValid)
            {
                LoadDropdowns();
                return View(model);
            }

            // Tìm sách cần sửa
            var sach = _db.Saches.FirstOrDefault(x => x.MaSach == model.MaSach);
            if (sach == null) return NotFound();

            // Cập nhật dữ liệu
            sach.TenSach = model.TenSach;
            sach.MaChuDe = model.MaChuDe;
            sach.MaTacGia = model.MaTacGia;
            sach.MaNhaXuatBan = model.MaNhaXuatBan;
            sach.GiaBan = model.GiaBan;
            sach.SoLuong = model.SoLuong;
            sach.NhaCungCap = model.NhaCungCap;
            sach.MoTa = model.MoTa;
            sach.NgayCapNhat = DateTime.Now;

            // Upload ảnh bìa nếu có
            if (fileAnhBia != null && fileAnhBia.Length > 0)
            {
                string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "uploads");

                // Tạo thư mục uploads nếu chưa tồn tại
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                // Xóa ảnh cũ nếu tồn tại
                if (!string.IsNullOrEmpty(sach.AnhBiaChinh))
                {
                    string oldFilePath = Path.Combine(uploadsFolder, sach.AnhBiaChinh);
                    if (System.IO.File.Exists(oldFilePath))
                    {
                        System.IO.File.Delete(oldFilePath);
                    }
                }

                string fileName = Guid.NewGuid().ToString() + Path.GetExtension(fileAnhBia.FileName);
                string filePath = Path.Combine(uploadsFolder, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    fileAnhBia.CopyTo(stream);
                }

                sach.AnhBiaChinh = fileName;
            }

            // Cập nhật vào DB
            _db.Saches.Update(sach);
            _db.SaveChanges();

            TempData["Success"] = "Cập nhật sách thành công!";
            return RedirectToAction("QuanLySach");
        }

        // ❌ Xóa sách
        public IActionResult XoaSach(int id)
        {
            var sach = _db.Saches.FirstOrDefault(x => x.MaSach == id);
            if (sach == null) return NotFound();

            // Xóa ảnh nếu tồn tại
            if (!string.IsNullOrEmpty(sach.AnhBiaChinh))
            {
                string filePath = Path.Combine(_webHostEnvironment.WebRootPath, "uploads", sach.AnhBiaChinh);
                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                }
            }

            _db.Saches.Remove(sach);
            _db.SaveChanges();

            TempData["Success"] = "Xóa sách thành công!";
            return RedirectToAction("QuanLySach");
        }

        // ========== QUẢN LÝ THỂ LOẠI ==========
        public IActionResult QuanLyTheLoai()
        {
            var theLoais = _db.ChuDes.ToList();
            return View(theLoais);
        }

        public IActionResult ThemTheLoai()
        {
            return View();
        }

        [HttpPost]
        public IActionResult ThemTheLoai(ChuDe model)
        {
            if (ModelState.IsValid)
            {
                _db.ChuDes.Add(model);
                _db.SaveChanges();
                return RedirectToAction("QuanLyTheLoai");
            }
            return View(model);
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

            // Tính tổng số sách từ các tác giả
            var tongSach = _db.Saches.Count();
            ViewBag.TongSach = tongSach;
            ViewData["CurrentFilter"] = searchString;

            return View(await tacGias.AsNoTracking().ToListAsync());
        }

        // GET: Thêm tác giả
        public IActionResult ThemTacGia()
        {
            return View();
        }

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

                // Nếu ModelState không valid, trả về view với lỗi
                return View(tacGia);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Lỗi khi thêm tác giả: " + ex.Message);
                return View(tacGia);
            }
        }

        // GET: Sửa tác giả
        [HttpGet]
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
            // 🔥 DEBUG CHI TIẾT
            Console.WriteLine($"=== 🎯 CONTROLLER: SuaTacGia POST STARTED ===");
            Console.WriteLine($"ID from route: {id}");
            Console.WriteLine($"ID from model: {tacGia.MaTacGia}");
            Console.WriteLine($"TenTG: {tacGia.TenTG}");
            Console.WriteLine($"QuocTich: {tacGia.QuocTich}");
            Console.WriteLine($"TieuSu: {tacGia.TieuSu}");
            Console.WriteLine($"ModelState IsValid: {ModelState.IsValid}");

            // Kiểm tra ID có khớp không
            if (id != tacGia.MaTacGia)
            {
                Console.WriteLine($"❌ ID MISMATCH: Route {id} != Model {tacGia.MaTacGia}");
                TempData["Error"] = "ID không khớp!";
                return View(tacGia);
            }

            // Debug ModelState errors
            if (!ModelState.IsValid)
            {
                Console.WriteLine("=== ❌ MODEL STATE ERRORS ===");
                foreach (var key in ModelState.Keys)
                {
                    var state = ModelState[key];
                    foreach (var error in state.Errors)
                    {
                        Console.WriteLine($"Key: {key}, Error: {error.ErrorMessage}");
                    }
                }

                // Trả về view với model để hiển thị lỗi
                return View(tacGia);
            }

            try
            {
                Console.WriteLine("=== 🔍 KIỂM TRA DỮ LIỆU ==");

                // Kiểm tra tên tác giả đã tồn tại chưa (trừ chính nó)
                var existingTacGia = await _db.TacGias
                    .FirstOrDefaultAsync(tg => tg.TenTG.ToLower() == tacGia.TenTG.ToLower() && tg.MaTacGia != id);

                if (existingTacGia != null)
                {
                    Console.WriteLine($"❌ TÊN TRÙNG: {tacGia.TenTG} đã tồn tại với ID {existingTacGia.MaTacGia}");
                    ModelState.AddModelError("TenTG", "Tên tác giả đã tồn tại trong hệ thống");
                    TempData["Error"] = "Tên tác giả đã tồn tại trong hệ thống";
                    return View(tacGia);
                }

                // Lấy tác giả hiện tại từ database
                var existing = await _db.TacGias.FindAsync(id);
                if (existing == null)
                {
                    Console.WriteLine($"❌ KHÔNG TÌM THẤY: Tác giả với ID {id} không tồn tại");
                    return NotFound();
                }

                Console.WriteLine("=== ✏️ CẬP NHẬT DỮ LIỆU ===");
                Console.WriteLine($"Từ: {existing.TenTG} -> {tacGia.TenTG}");

                // Cập nhật thông tin
                existing.TenTG = tacGia.TenTG.Trim();
                existing.QuocTich = tacGia.QuocTich?.Trim() ?? string.Empty;
                existing.TieuSu = tacGia.TieuSu?.Trim() ?? string.Empty;

                _db.TacGias.Update(existing);
                await _db.SaveChangesAsync();

                Console.WriteLine("✅ CẬP NHẬT THÀNH CÔNG");
                TempData["Success"] = "Cập nhật tác giả thành công!";
                return RedirectToAction(nameof(QuanLyTacGia));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ EXCEPTION: {ex.Message}");
                Console.WriteLine($"Stack Trace: {ex.StackTrace}");
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

        // ========== QUẢN LÝ NHÀ XUẤT BẢN ==========
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
        public IActionResult ThemNXB(NhaXuatBan model)
        {
            if (ModelState.IsValid)
            {
                _db.NhaXuatBans.Add(model);
                _db.SaveChanges();
                TempData["Success"] = "Thêm nhà xuất bản thành công!";
                return RedirectToAction("QuanLyNXB");
            }
            return View(model);
        }

        public IActionResult SuaNXB(int id)
        {
            var nxb = _db.NhaXuatBans.Find(id);
            if (nxb == null) return NotFound();
            return View(nxb);
        }

        [HttpPost]
        public IActionResult SuaNXB(NhaXuatBan model)
        {
            if (ModelState.IsValid)
            {
                _db.NhaXuatBans.Update(model);
                _db.SaveChanges();
                TempData["Success"] = "Cập nhật nhà xuất bản thành công!";
                return RedirectToAction("QuanLyNXB");
            }
            return View(model);
        }

        public IActionResult XoaNXB(int id)
        {
            var nxb = _db.NhaXuatBans.Find(id);
            if (nxb == null) return NotFound();

            // Kiểm tra xem có sách nào thuộc NXB này không
            var coSach = _db.Saches.Any(s => s.MaNhaXuatBan == id);
            var soSach = _db.Saches.Count(s => s.MaNhaXuatBan == id);

            if (coSach)
            {
                TempData["Error"] = $"Không thể xóa nhà xuất bản '{nxb.TenNXB}' vì có {soSach} sách thuộc nhà xuất bản này!";
                return RedirectToAction("QuanLyNXB");
            }

            _db.NhaXuatBans.Remove(nxb);
            _db.SaveChanges();
            TempData["Success"] = "Xóa nhà xuất bản thành công!";
            return RedirectToAction("QuanLyNXB");
        }
    }
}