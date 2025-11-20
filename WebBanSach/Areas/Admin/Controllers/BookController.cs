using Media.Models;
using Meida.DataAccess.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.IO;
using System.Threading.Tasks;

namespace ProjectCuoiKi.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class BookController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _environment;

        public BookController(ApplicationDbContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }

        // GET: Sửa sách
        [HttpGet]
        public async Task<IActionResult> SuaSach(int id)
        {
            try
            {
                var sach = await _context.Saches
                    .Include(s => s.ChuDe)
                    .Include(s => s.TacGia)
                    .Include(s => s.NhaXuatBan)
                    .FirstOrDefaultAsync(s => s.MaSach == id);

                if (sach == null)
                {
                    TempData["Error"] = "Không tìm thấy sách";
                    return RedirectToAction("QuanLySach");
                }

                // Load dropdown data
                await LoadDropdownData();
                return View(sach);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Lỗi khi tải thông tin sách: " + ex.Message;
                return RedirectToAction("QuanLySach");
            }
        }

        // POST: Sửa sách
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SuaSach(int id, Sach sach, IFormFile fileAnhBiaChinh, List<IFormFile> filesAnhBiaPhu)
        {
            try
            {
                if (id != sach.MaSach)
                {
                    TempData["Error"] = "ID sách không khớp";
                    return RedirectToAction("QuanLySach");
                }

                // Tắt validation cho navigation properties
                ModelState.Remove("ChuDe");
                ModelState.Remove("TacGia");
                ModelState.Remove("NhaXuatBan");

                if (ModelState.IsValid)
                {
                    var existingSach = await _context.Saches.FindAsync(id);
                    if (existingSach == null)
                    {
                        TempData["Error"] = "Không tìm thấy sách";
                        return RedirectToAction("QuanLySach");
                    }

                    // Cập nhật thông tin cơ bản
                    existingSach.TenSach = sach.TenSach;
                    existingSach.GiaBan = sach.GiaBan;
                    existingSach.PhanTramGiamGia = sach.PhanTramGiamGia;
                    existingSach.SoLuong = sach.SoLuong;
                    existingSach.NhaCungCap = sach.NhaCungCap;
                    existingSach.MoTa = sach.MoTa;
                    existingSach.MaChuDe = sach.MaChuDe;
                    existingSach.MaTacGia = sach.MaTacGia;
                    existingSach.MaNhaXuatBan = sach.MaNhaXuatBan;
                    existingSach.NgayCapNhat = DateTime.Now;

                    // Xử lý ảnh bìa chính
                    if (fileAnhBiaChinh != null && fileAnhBiaChinh.Length > 0)
                    {
                        var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "img", "product");
                        if (!Directory.Exists(uploadsFolder))
                        {
                            Directory.CreateDirectory(uploadsFolder);
                        }

                        var fileName = Guid.NewGuid().ToString() + Path.GetExtension(fileAnhBiaChinh.FileName);
                        var filePath = Path.Combine(uploadsFolder, fileName);

                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await fileAnhBiaChinh.CopyToAsync(stream);
                        }

                        // Xóa ảnh cũ nếu có
                        if (!string.IsNullOrEmpty(existingSach.AnhBiaChinh))
                        {
                            var oldImagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", existingSach.AnhBiaChinh.TrimStart('/'));
                            if (System.IO.File.Exists(oldImagePath))
                            {
                                System.IO.File.Delete(oldImagePath);
                            }
                        }

                        existingSach.AnhBiaChinh = "/img/product/" + fileName;
                    }

                    // Xử lý ảnh bìa phụ
                    if (filesAnhBiaPhu != null && filesAnhBiaPhu.Count > 0)
                    {
                        var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "img", "product");
                        if (!Directory.Exists(uploadsFolder))
                        {
                            Directory.CreateDirectory(uploadsFolder);
                        }

                        // Reset ảnh bìa phụ
                        existingSach.AnhBiaPhu1 = null;
                        existingSach.AnhBiaPhu2 = null;
                        existingSach.AnhBiaPhu3 = null;
                        existingSach.AnhBiaPhu4 = null;

                        // Xóa ảnh cũ
                        DeleteOldImages(existingSach);

                        // Upload ảnh mới (tối đa 4 ảnh)
                        for (int i = 0; i < Math.Min(filesAnhBiaPhu.Count, 4); i++)
                        {
                            var file = filesAnhBiaPhu[i];
                            if (file != null && file.Length > 0)
                            {
                                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                                var filePath = Path.Combine(uploadsFolder, fileName);

                                using (var stream = new FileStream(filePath, FileMode.Create))
                                {
                                    await file.CopyToAsync(stream);
                                }

                                // Gán vào các property tương ứng
                                switch (i)
                                {
                                    case 0:
                                        existingSach.AnhBiaPhu1 = "/img/product/" + fileName;
                                        break;
                                    case 1:
                                        existingSach.AnhBiaPhu2 = "/img/product/" + fileName;
                                        break;
                                    case 2:
                                        existingSach.AnhBiaPhu3 = "/img/product/" + fileName;
                                        break;
                                    case 3:
                                        existingSach.AnhBiaPhu4 = "/img/product/" + fileName;
                                        break;
                                }
                            }
                        }
                    }

                    _context.Saches.Update(existingSach);
                    await _context.SaveChangesAsync();

                    TempData["Success"] = "Cập nhật sách thành công!";
                    return RedirectToAction("QuanLySach");
                }
                else
                {
                    // Log lỗi validation
                    var errors = ModelState.Values.SelectMany(v => v.Errors);
                    foreach (var error in errors)
                    {
                        Console.WriteLine($"Validation Error: {error.ErrorMessage}");
                    }

                    TempData["Error"] = "Dữ liệu không hợp lệ. Vui lòng kiểm tra lại.";
                }
            }
            catch (DbUpdateException dbEx)
            {
                TempData["Error"] = $"Lỗi database: {dbEx.InnerException?.Message ?? dbEx.Message}";
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Lỗi: {ex.Message}";
            }

            // Load lại dropdown data nếu có lỗi
            await LoadDropdownData();
            return View(sach);
        }

        // Hàm xóa ảnh cũ
        private void DeleteOldImages(Sach sach)
        {
            var imageProperties = new[] { sach.AnhBiaPhu1, sach.AnhBiaPhu2, sach.AnhBiaPhu3, sach.AnhBiaPhu4 };

            foreach (var imagePath in imageProperties)
            {
                if (!string.IsNullOrEmpty(imagePath))
                {
                    var fullPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", imagePath.TrimStart('/'));
                    if (System.IO.File.Exists(fullPath))
                    {
                        System.IO.File.Delete(fullPath);
                    }
                }
            }
        }
        // GET: Thêm sách
        public async Task<IActionResult> ThemSach()
        {
            try
            {
                // Load dropdown data
                ViewBag.ChuDes = await _context.ChuDes.ToListAsync();
                ViewBag.TacGias = await _context.TacGias.ToListAsync();
                ViewBag.NhaXuatBans = await _context.NhaXuatBans.ToListAsync();

                var model = new Sach();
                return View(model);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Lỗi khi tải trang: " + ex.Message;
                return View(new Sach());
            }
        }

        // POST: Thêm sách
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ThemSach(Sach sach, IFormFile fileAnhBiaChinh)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    // Xử lý upload ảnh - SỬA ĐƯỜNG DẪN THÀNH img/product
                    if (fileAnhBiaChinh != null && fileAnhBiaChinh.Length > 0)
                    {
                        // Sửa đường dẫn thành img/product
                        var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "img", "product");
                        if (!Directory.Exists(uploadsFolder))
                        {
                            Directory.CreateDirectory(uploadsFolder);
                        }

                        var fileName = Guid.NewGuid().ToString() + Path.GetExtension(fileAnhBiaChinh.FileName);
                        var filePath = Path.Combine(uploadsFolder, fileName);

                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await fileAnhBiaChinh.CopyToAsync(stream);
                        }

                        // Sửa đường dẫn ảnh thành /img/product/
                        sach.AnhBiaChinh = "/img/product/" + fileName;
                    }

                    // Đảm bảo các giá trị mặc định
                    sach.NgayCapNhat = DateTime.Now;

                    _context.Saches.Add(sach);
                    await _context.SaveChangesAsync();

                    TempData["Success"] = "Thêm sách thành công!";
                    return RedirectToAction("QuanLySach");
                }
                else
                {
                    // Log lỗi validation
                    var errors = ModelState.Values.SelectMany(v => v.Errors);
                    foreach (var error in errors)
                    {
                        Console.WriteLine($"Validation Error: {error.ErrorMessage}");
                    }

                    TempData["Error"] = "Dữ liệu không hợp lệ. Vui lòng kiểm tra lại.";
                }
            }
            catch (DbUpdateException dbEx)
            {
                TempData["Error"] = $"Lỗi database: {dbEx.InnerException?.Message ?? dbEx.Message}";
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Lỗi: {ex.Message}";
            }

            // Load lại dropdown data nếu có lỗi
            LoadViewData();
            return View(sach);
        }

        private void LoadViewData()
        {
            ViewBag.ChuDes = _context.ChuDes.ToList();
            ViewBag.TacGias = _context.TacGias.ToList();
            ViewBag.NhaXuatBans = _context.NhaXuatBans.ToList();
        }
        private async Task LoadDropdownData()
        {
            ViewBag.ChuDes = await _context.ChuDes.ToListAsync();
            ViewBag.TacGias = await _context.TacGias.ToListAsync();
            ViewBag.NhaXuatBans = await _context.NhaXuatBans.ToListAsync();
        }

        private async Task<string> SaveImage(IFormFile file)
        {
            try
            {
                // Tạo thư mục nếu chưa tồn tại
                var uploadsFolder = Path.Combine(_environment.WebRootPath, "images", "books");
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                // Tạo tên file unique
                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                var filePath = Path.Combine(uploadsFolder, fileName);

                // Lưu file
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                // Trả về đường dẫn tương đối
                return $"/images/books/{fileName}";
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi khi lưu ảnh: {ex.Message}");
                throw;
            }
        }

        // GET: Quản lý sách
        public async Task<IActionResult> QuanLySach()
        {
            var saches = await _context.Saches
                .Include(s => s.TacGia)
                .Include(s => s.NhaXuatBan)
                .Include(s => s.ChuDe)
                .ToListAsync();
            return View(saches);
        }
        // GET: Xóa sách
        [HttpGet]
        public async Task<IActionResult> XoaSach(int id)
        {
            try
            {
                var sach = await _context.Saches.FindAsync(id);
                if (sach == null)
                {
                    TempData["Error"] = "Không tìm thấy sách";
                    return RedirectToAction("QuanLySach");
                }

                // Xóa ảnh bìa chính
                if (!string.IsNullOrEmpty(sach.AnhBiaChinh))
                {
                    var mainImagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", sach.AnhBiaChinh.TrimStart('/'));
                    if (System.IO.File.Exists(mainImagePath))
                    {
                        System.IO.File.Delete(mainImagePath);
                    }
                }

                // Xóa ảnh bìa phụ
                DeleteOldImages(sach);

                // Xóa sách khỏi database
                _context.Saches.Remove(sach);
                await _context.SaveChangesAsync();

                TempData["Success"] = "Xóa sách thành công!";
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Lỗi khi xóa sách: {ex.Message}";
            }

            return RedirectToAction("QuanLySach");
        }

        // GET: Chi tiết sách
        [HttpGet]
        public async Task<IActionResult> ChiTietSach(int id)
        {
            try
            {
                var sach = await _context.Saches
                    .Include(s => s.TacGia)
                    .Include(s => s.NhaXuatBan)
                    .Include(s => s.ChuDe)
                    .FirstOrDefaultAsync(s => s.MaSach == id);

                if (sach == null)
                {
                    TempData["Error"] = "Không tìm thấy sách";
                    return RedirectToAction("QuanLySach");
                }

                return View(sach);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Lỗi khi tải thông tin sách: " + ex.Message;
                return RedirectToAction("QuanLySach");
            }
        }

    }

}