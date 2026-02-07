using Media.Areas.Admin.Controllers;
using Media.Models;
using Meida.DataAccess.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.IO;
using System.Threading.Tasks;

namespace ProjectCuoiKi.Areas.Admin.Controllers
{
    public class BookController : AdminController
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


                ModelState.Remove("ChuDe");
                ModelState.Remove("TacGia");
                ModelState.Remove("NhaXuatBan");
                ModelState.Remove("fileAnhBiaChinh");
                ModelState.Remove("filesAnhBiaPhu");

                if (ModelState.IsValid)
                {
                    var existingSach = await _context.Saches.FindAsync(id);
                    if (existingSach == null)
                    {
                        TempData["Error"] = "Không tìm thấy sách";
                        return RedirectToAction("QuanLySach");
                    }

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

                    if (fileAnhBiaChinh != null && fileAnhBiaChinh.Length > 0)
                    {
                        var uploadsFolder = Path.Combine(_environment.WebRootPath, "img", "product");
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

                        if (!string.IsNullOrEmpty(existingSach.AnhBiaChinh))
                        {
                            var oldImagePath = Path.Combine(_environment.WebRootPath, existingSach.AnhBiaChinh.TrimStart('/'));
                            if (System.IO.File.Exists(oldImagePath))
                            {
                                System.IO.File.Delete(oldImagePath);
                            }
                        }

                        existingSach.AnhBiaChinh = fileName;
                    }

                    if (filesAnhBiaPhu != null && filesAnhBiaPhu.Count > 0)
                    {
                        var uploadsFolder = Path.Combine(_environment.WebRootPath, "img", "product");
                        if (!Directory.Exists(uploadsFolder))
                        {
                            Directory.CreateDirectory(uploadsFolder);
                        }
                        var oldImages = new List<string>
                        {
                            existingSach.AnhBiaPhu1,
                            existingSach.AnhBiaPhu2,
                            existingSach.AnhBiaPhu3,
                            existingSach.AnhBiaPhu4
                        };

                        existingSach.AnhBiaPhu1 = null;
                        existingSach.AnhBiaPhu2 = null;
                        existingSach.AnhBiaPhu3 = null;
                        existingSach.AnhBiaPhu4 = null;

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

                                switch (i)
                                {
                                    case 0:
                                        existingSach.AnhBiaPhu1 = fileName;
                                        break;
                                    case 1:
                                        existingSach.AnhBiaPhu2 = fileName;
                                        break;
                                    case 2:
                                        existingSach.AnhBiaPhu3 = fileName;
                                        break;
                                    case 3:
                                        existingSach.AnhBiaPhu4 = fileName;
                                        break;
                                }
                            }
                        }

                        foreach (var oldImage in oldImages)
                        {
                            if (!string.IsNullOrEmpty(oldImage))
                            {
                                var oldImagePath = Path.Combine(_environment.WebRootPath, "img", "product", oldImage);
                                if (System.IO.File.Exists(oldImagePath))
                                {
                                    System.IO.File.Delete(oldImagePath);
                                }
                            }
                        }
                    }

                    existingSach.PhanTramGiamGia /= 100;
                    _context.Saches.Update(existingSach);
                    await _context.SaveChangesAsync();

                    TempData["Success"] = "Cập nhật sách thành công!";
                    return RedirectToAction("QuanLySach");
                }
                else
                {
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

            await LoadDropdownData();
            return View(sach);
        }

        private void DeleteOldImages(Sach sach)
        {
            var imageProperties = new[] { sach.AnhBiaPhu1, sach.AnhBiaPhu2, sach.AnhBiaPhu3, sach.AnhBiaPhu4 };

            foreach (var imageName in imageProperties)
            {
                if (!string.IsNullOrEmpty(imageName))
                {
                    var fullPath = Path.Combine(_environment.WebRootPath, "img", "product", imageName);
                    if (System.IO.File.Exists(fullPath))
                    {
                        System.IO.File.Delete(fullPath);
                    }
                }
            }
        }

        public async Task<IActionResult> ThemSach()
        {
            try
            {
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ThemSach(Sach sach, IFormFile fileAnhBiaChinh)
        {
            try
            {
                ModelState.Remove("fileAnhBiaChinh");

                if (ModelState.IsValid)
                {
                    if (fileAnhBiaChinh != null && fileAnhBiaChinh.Length > 0)
                    {
                        var uploadsFolder = Path.Combine(_environment.WebRootPath, "img", "product");
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

                        sach.AnhBiaChinh = fileName;
                    }

                    sach.NgayCapNhat = DateTime.Now;
                    sach.PhanTramGiamGia /= 100;

                    _context.Saches.Add(sach);
                    await _context.SaveChangesAsync();

                    TempData["Success"] = "Thêm sách thành công!";
                    return RedirectToAction("QuanLySach");
                }
                else
                {
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

        public async Task<IActionResult> QuanLySach(string search = "", int page = 1)
        {
            try
            {
                var pageSize = 10;

                var allBooksQuery = _context.Saches
                    .Include(s => s.TacGia)
                    .Include(s => s.NhaXuatBan)
                    .Include(s => s.ChuDe)
                    .AsQueryable();

                if (!string.IsNullOrEmpty(search))
                {
                    search = search.ToLower();
                    allBooksQuery = allBooksQuery.Where(s =>
                        s.TenSach.ToLower().Contains(search) ||
                        (s.TacGia != null && s.TacGia.TenTG.ToLower().Contains(search)) ||
                        (s.NhaXuatBan != null && s.NhaXuatBan.TenNXB.ToLower().Contains(search)) ||
                        (s.ChuDe != null && s.ChuDe.TenChuDe.ToLower().Contains(search)) ||
                        s.NhaCungCap.ToLower().Contains(search)
                    );
                }

                var allBooks = await allBooksQuery.ToListAsync();

                var totalBooks = allBooks.Count;
                var availableBooks = allBooks.Count(s => s.SoLuong > 0);
                var totalQuantity = allBooks.Sum(s => s.SoLuong);
                var totalValue = allBooks.Sum(s => s.GiaBan * (decimal)s.SoLuong);

                var pagedBooks = allBooks
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();

                ViewBag.TotalBooks = totalBooks;
                ViewBag.AvailableBooks = availableBooks;
                ViewBag.TotalQuantity = totalQuantity;
                ViewBag.TotalValue = totalValue;
                ViewBag.CurrentPage = page;
                ViewBag.TotalPages = (int)Math.Ceiling(totalBooks / (double)pageSize);
                ViewBag.Search = search;

                return View(pagedBooks);
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Lỗi khi tải danh sách sách: {ex.Message}";
                return View(new List<Sach>());
            }
        }

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
                    var mainImagePath = Path.Combine(_environment.WebRootPath, "img", "product", sach.AnhBiaChinh);
                    if (System.IO.File.Exists(mainImagePath))
                    {
                        System.IO.File.Delete(mainImagePath);
                    }
                }

                // Xóa ảnh bìa phụ
                DeleteOldImages(sach);

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