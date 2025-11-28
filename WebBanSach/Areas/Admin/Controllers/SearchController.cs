using Media.Models;
using Media.Models.ViewModels;
using Meida.DataAccess.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using OfficeOpenXml;

namespace ProjectCuoiKi.Areas.Admin.Controllers
{
    // Tạo Controller mới
    [Area("Admin")]
    public class SearchController : Controller
    {
        private readonly ApplicationDbContext _context;

        public SearchController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GlobalSearch(string term)
        {
            if (string.IsNullOrWhiteSpace(term))
                return Json(new List<SearchResult>());

            var results = new List<SearchResult>();

            try
            {
                // 1. TÌM SÁCH
                var books = await _context.Saches
                    .Include(s => s.TacGia)
                    .Include(s => s.ChuDe)
                    .Include(s => s.NhaXuatBan)
                    .Where(s => s.TenSach.Contains(term) ||
                               s.TacGia.TenTG.Contains(term) ||
                               s.ChuDe.TenChuDe.Contains(term) ||
                               s.NhaXuatBan.TenNXB.Contains(term))
                    .Select(s => new SearchResult
                    {
                        Id = s.MaSach,
                        Type = "book",
                        Title = s.TenSach,
                        Description = $"{s.TacGia.TenTG} - {s.ChuDe.TenChuDe}",
                        Extra = $"Giá: {s.GiaSauGiam.ToString("N0")}₫ - Tồn kho: {s.SoLuong}"
                    })
                    .Take(5)
                    .ToListAsync();

                results.AddRange(books);

                // 2. TÌM ĐƠN HÀNG
                var orders = await _context.DonHangs
                    .Include(o => o.KhachHang)
                    .Where(o => o.MaDonHang.ToString().Contains(term) ||
                               o.TenNguoiNhan.Contains(term) ||
                               o.SoDienThoaiNhan.Contains(term) ||
                               o.KhachHang.HoTen.Contains(term) ||
                               o.KhachHang.Email.Contains(term))
                    .Select(o => new SearchResult
                    {
                        Id = o.MaDonHang,
                        Type = "order",
                        Title = $"Đơn hàng #{o.MaDonHang}",
                        Description = $"{o.TenNguoiNhan} - {o.SoDienThoaiNhan}",
                        Extra = $"{o.Total.ToString("N0")}₫ - {o.NgayTao:dd/MM/yyyy} - {(o.DaThanhToan ? "Đã thanh toán" : "Chưa thanh toán")}"
                    })
                    .Take(5)
                    .ToListAsync();

                results.AddRange(orders);

                // 3. TÌM KHÁCH HÀNG
                var customers = await _context.KhachHangs
                    .Where(c => c.HoTen.Contains(term) ||
                               c.DienThoai.Contains(term) ||
                               c.Email.Contains(term) ||
                               c.MaTaiKhoan.Contains(term))
                    .Select(c => new SearchResult
                    {
                        Id = c.MaKhachHang,
                        Type = "user",
                        Title = c.HoTen,
                        Description = c.Email,
                        Extra = $"{c.DienThoai}"
                    })
                    .Take(5)
                    .ToListAsync();

                results.AddRange(customers);

                // 4. TÌM TÁC GIẢ
                var authors = await _context.TacGias
                    .Where(a => a.TenTG.Contains(term))
                    .Select(a => new SearchResult
                    {
                        Id = a.MaTacGia,
                        Type = "author",
                        Title = a.TenTG,
                        Description = a.QuocTich ?? "Chưa cập nhật quốc tịch",
                        Extra = a.TieuSu != null ? (a.TieuSu.Length > 50 ? a.TieuSu.Substring(0, 50) + "..." : a.TieuSu) : "Chưa có tiểu sử"
                    })
                    .Take(5)
                    .ToListAsync();

                results.AddRange(authors);

                // 5. TÌM CHỦ ĐỀ
                var categories = await _context.ChuDes
                    .Where(c => c.TenChuDe.Contains(term))
                    .Select(c => new SearchResult
                    {
                        Id = c.MaChuDe,
                        Type = "user",
                        Title = c.TenChuDe,
                        Description = "Thể loại sách",
                        Extra = $"Slug: {c.Slug}"
                    })
                    .Take(5)
                    .ToListAsync();

                results.AddRange(categories);

                return Json(results);
            }
            catch (Exception ex)
            {
                // Log lỗi
                Console.WriteLine($"Search error: {ex.Message}");
                return Json(new List<SearchResult>());
            }
        }
    }

    public class SearchResult
    {
        public int Id { get; set; }
        public string Type { get; set; } // 'book', 'order', 'user', 'author', 'category'
        public string Title { get; set; }
        public string Description { get; set; }
        public string Extra { get; set; }
    }
}
