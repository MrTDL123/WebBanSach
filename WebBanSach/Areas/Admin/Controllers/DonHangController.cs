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

                // Status filter
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
            if (id == null) return NotFound();

            var donHang = await _context.DonHangs
                .Include(dh => dh.KhachHang)
                .Include(dh => dh.NhanVien)
                .Include(dh => dh.VanChuyen)
                .Include(dh => dh.DiaChiNhanHang)
                .Include(dh => dh.HoaDon)
                .Include(dh => dh.ChiTietDonHangs)
                    .ThenInclude(ct => ct.Sach)
                .FirstOrDefaultAsync(m => m.MaDonHang == id);

            if (donHang == null) return NotFound();

            return View(donHang);
        }


        // GET: DonHang/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            Console.WriteLine($"=== DEBUG: Edit called with id = {id} ===");

            if (id == null)
            {
                Console.WriteLine("ID is null");
                return NotFound();
            }

            try
            {
                // Debug: Kiểm tra xem có đơn hàng nào trong DB không
                var allOrders = await _context.DonHangs.Select(dh => dh.MaDonHang).ToListAsync();
                Console.WriteLine($"All order IDs in DB: {string.Join(", ", allOrders)}");
                Console.WriteLine($"Looking for order with ID: {id}");

                var donHang = await _context.DonHangs
                    .Include(dh => dh.ChiTietDonHangs)
                        .ThenInclude(ct => ct.Sach)
                    .Include(dh => dh.KhachHang)
                    .Include(dh => dh.NhanVien)
                    .Include(dh => dh.VanChuyen)
                    .Include(dh => dh.DiaChiNhanHang)
                    .AsNoTracking()
                    .FirstOrDefaultAsync(m => m.MaDonHang == id);

                if (donHang == null)
                {
                    Console.WriteLine($"Order with ID {id} NOT FOUND in database");
                    TempData["Error"] = $"Không tìm thấy đơn hàng #{id}";
                    return RedirectToAction(nameof(QuanLyDonHang));
                }

                Console.WriteLine($"Order found: #{donHang.MaDonHang}, Customer: {donHang.KhachHang?.HoTen}");
                LoadViewData();
                return View(donHang);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception in Edit: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                TempData["Error"] = $"Lỗi: {ex.Message}";
                return RedirectToAction(nameof(QuanLyDonHang));
            }
        }
        // POST: DonHang/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, DonHang donHang)
        {
            if (id != donHang.MaDonHang)
            {
                TempData["Error"] = "ID đơn hàng không khớp!";
                return RedirectToAction(nameof(QuanLyDonHang));
            }

            try
            {
                // Lấy đơn hàng hiện tại từ database
                var existingDonHang = await _context.DonHangs
                    .Include(dh => dh.ChiTietDonHangs)
                    .Include(dh => dh.VanChuyen)
                    .FirstOrDefaultAsync(dh => dh.MaDonHang == id);

                if (existingDonHang == null)
                {
                    TempData["Error"] = "Không tìm thấy đơn hàng!";
                    return RedirectToAction(nameof(QuanLyDonHang));
                }

                // Chỉ cập nhật các thông tin cho phép
                existingDonHang.MaNhanVien = donHang.MaNhanVien;
                existingDonHang.TenNguoiNhan = donHang.TenNguoiNhan;
                existingDonHang.SoDienThoaiNhan = donHang.SoDienThoaiNhan;
                existingDonHang.DiaChiChiTiet = donHang.DiaChiChiTiet;
                existingDonHang.PhuongXa = donHang.PhuongXa;
                existingDonHang.QuanHuyen = donHang.QuanHuyen;
                existingDonHang.TinhThanh = donHang.TinhThanh;
                existingDonHang.HinhThucThanhToan = donHang.HinhThucThanhToan;
                existingDonHang.DaThanhToan = donHang.DaThanhToan;
                existingDonHang.GhiChu = donHang.GhiChu;
                existingDonHang.NgayCapNhat = DateTime.Now; // CẬP NHẬT NGÀY SỬA

                // Cập nhật tổng tiền từ chi tiết đơn hàng
                existingDonHang.Total = existingDonHang.ChiTietDonHangs?.Sum(ct => ct.ThanhTien) ?? 0;

                _context.Update(existingDonHang);
                await _context.SaveChangesAsync();

                TempData["Success"] = "Cập nhật đơn hàng thành công!";
                return RedirectToAction(nameof(QuanLyDonHang));
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Lỗi khi cập nhật đơn hàng: " + ex.Message;
                LoadViewData();
                return View(donHang);
            }
        }

        // GET: Lấy thông tin vận chuyển
        [HttpGet]
        public async Task<IActionResult> GetShippingInfo(int id)
        {
            try
            {
                var vanChuyen = await _context.VanChuyens
                    .FirstOrDefaultAsync(vc => vc.MaDonHang == id);

                if (vanChuyen == null)
                {
                    return Json(new { exists = false, message = "Chưa có thông tin vận chuyển" });
                }

                return Json(new
                {
                    exists = true,
                    donViVanChuyen = vanChuyen.DonViVanChuyen,
                    maVanDon = vanChuyen.MaVanDon,
                    trangThaiGiaoHang = vanChuyen.TrangThaiGiaoHang.ToString()
                });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Lỗi: " + ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> UpdateShippingStatus([FromBody] CapNhatTrangThaiGiaoHangVM data)
        {
            try
            {
                Console.WriteLine($"=== DEBUG: Received data - ID: {data?.id}, TrangThai: {data?.trangThai} ===");

                if (data == null)
                {
                    return Json(new { success = false, message = "Dữ liệu không hợp lệ" });
                }

                // Lấy dữ liệu từ body JSON
                int id = data.id;
                string trangThaiString = data.trangThai; // NHẬN string từ client

                Console.WriteLine($"Processing - ID: {id}, TrangThaiString: {trangThaiString}");

                // Parse string sang enum
                if (!Enum.TryParse<TrangThaiGiaoHang>(trangThaiString, out TrangThaiGiaoHang trangThaiEnum))
                {
                    Console.WriteLine($"Failed to parse TrangThai: {trangThaiString}");
                    return Json(new { success = false, message = $"Trạng thái không hợp lệ: {trangThaiString}" });
                }

                Console.WriteLine($"Parsed enum: {trangThaiEnum}");

                // Tìm đơn hàng
                var donHang = await _context.DonHangs
                    .Include(dh => dh.VanChuyen)
                    .FirstOrDefaultAsync(dh => dh.MaDonHang == id);

                if (donHang == null)
                {
                    Console.WriteLine($"Order #{id} not found");
                    return Json(new { success = false, message = $"Không tìm thấy đơn hàng #{id}" });
                }

                Console.WriteLine($"Found order: #{donHang.MaDonHang}");

                // Nếu chưa có vận chuyển thì tạo mới
                if (donHang.VanChuyen == null)
                {
                    Console.WriteLine("Creating new VanChuyen record");
                    donHang.VanChuyen = new VanChuyen
                    {
                        MaDonHang = id,
                        DonViVanChuyen = "Đang cập nhật",
                        MaVanDon = $"VC-{id}-{DateTime.Now:yyyyMMddHHmmss}",
                        PhiVanChuyen = 0,
                        TrangThaiGiaoHang = trangThaiEnum
                    };

                    _context.VanChuyens.Add(donHang.VanChuyen);
                }
                else
                {
                    Console.WriteLine($"Updating existing VanChuyen from {donHang.VanChuyen.TrangThaiGiaoHang} to {trangThaiEnum}");
                    // Cập nhật trạng thái
                    donHang.VanChuyen.TrangThaiGiaoHang = trangThaiEnum;
                    _context.VanChuyens.Update(donHang.VanChuyen);
                }

                // Kiểm tra xem có thay đổi không
                Console.WriteLine($"Has changes: {_context.ChangeTracker.HasChanges()}");

                var result = await _context.SaveChangesAsync();
                Console.WriteLine($"SaveChanges result: {result} rows affected");

                if (result > 0)
                {
                    return Json(new
                    {
                        success = true,
                        message = "Đã cập nhật trạng thái vận chuyển!",
                        newStatus = trangThaiEnum.ToString()
                    });
                }
                else
                {
                    return Json(new { success = false, message = "Không có thay đổi nào được lưu!" });
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"=== EXCEPTION: {ex.Message} ===");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                return Json(new { success = false, message = $"Lỗi server: {ex.Message}" });
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
        public async Task<IActionResult> RemoveChiTietDonHang(int maDonHang, int maSach)
        {
            try
            {
                // Tìm chi tiết bằng cả MaDonHang và MaSach
                var chiTiet = await _context.ChiTietDonHangs
                    .Include(ct => ct.DonHang)
                    .FirstOrDefaultAsync(ct => ct.MaDonHang == maDonHang && ct.MaSach == maSach);

                if (chiTiet != null)
                {
                    _context.ChiTietDonHangs.Remove(chiTiet);
                    await _context.SaveChangesAsync();

                    // Cập nhật tổng tiền đơn hàng
                    await UpdateTotalDonHang(maDonHang);

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

                // QUAN TRỌNG: Thêm dòng này để fix lỗi null
                ViewBag.Saches = new SelectList(_context.Saches, "MaSach", "TenSach");

                // SelectList cho hình thức thanh toán
                ViewBag.HinhThucThanhToan = new SelectList(
                    Enum.GetValues(typeof(HinhThucThanhToan))
                        .Cast<HinhThucThanhToan>()
                        .Select(e => new {
                            Value = e.ToString(),
                            Text = GetEnumDescription(e)
                        }),
                    "Value",
                    "Text"
                );

                // SelectList cho trạng thái vận chuyển
                ViewBag.TrangThaiGiaoHang = new SelectList(
                    Enum.GetValues(typeof(TrangThaiGiaoHang))
                        .Cast<TrangThaiGiaoHang>()
                        .Select(e => new {
                            Value = e.ToString(),
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
        [HttpGet]
        public async Task<IActionResult> CheckOrderExists(int id)
        {
            try
            {
                var exists = await _context.DonHangs.AnyAsync(dh => dh.MaDonHang == id);
                return Json(new { exists = exists, id = id });
            }
            catch (Exception ex)
            {
                return Json(new { exists = false, error = ex.Message });
            }
        }
        [HttpGet]
        public async Task<IActionResult> ExportToExcel(string search = "", string status = "")
        {
            try
            {
                // Lấy dữ liệu đơn hàng
                var query = _context.DonHangs
                    .Include(dh => dh.KhachHang)
                    .Include(dh => dh.NhanVien)
                    .Include(dh => dh.VanChuyen)
                    .Include(dh => dh.ChiTietDonHangs)
                        .ThenInclude(ct => ct.Sach)
                    .AsQueryable();

                // Filter search
                if (!string.IsNullOrEmpty(search))
                {
                    query = query.Where(dh =>
                        dh.MaDonHang.ToString().Contains(search) ||
                        dh.KhachHang.HoTen.Contains(search) ||
                        dh.SoDienThoaiNhan.Contains(search) ||
                        dh.TenNguoiNhan.Contains(search));
                }

                // Filter status
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

                var donHangs = await query.OrderByDescending(dh => dh.NgayTao).ToListAsync();

                // Tạo Excel
                using var package = new ExcelPackage();
                var worksheet = package.Workbook.Worksheets.Add("Đơn hàng");

                // Header
                var headers = new[]
                {
            "Mã đơn hàng", "Khách hàng", "Nhân viên", "Tên người nhận",
            "Số điện thoại", "Địa chỉ", "Tổng tiền", "Ngày tạo",
            "Thanh toán", "Trạng thái vận chuyển"
        };

                for (int i = 0; i < headers.Length; i++)
                {
                    worksheet.Cells[1, i + 1].Value = headers[i];
                    worksheet.Cells[1, i + 1].Style.Font.Bold = true;
                }

                // Data
                int row = 2;
                foreach (var dh in donHangs)
                {
                    worksheet.Cells[row, 1].Value = dh.MaDonHang;
                    worksheet.Cells[row, 2].Value = dh.KhachHang?.HoTen;
                    worksheet.Cells[row, 3].Value = dh.NhanVien?.HoTen;
                    worksheet.Cells[row, 4].Value = dh.TenNguoiNhan;
                    worksheet.Cells[row, 5].Value = dh.SoDienThoaiNhan;
                    worksheet.Cells[row, 6].Value = $"{dh.DiaChiChiTiet}, {dh.PhuongXa}, {dh.QuanHuyen}, {dh.TinhThanh}";
                    worksheet.Cells[row, 7].Value = dh.Total;
                    worksheet.Cells[row, 8].Value = dh.NgayTao.ToString("dd/MM/yyyy HH:mm");
                    worksheet.Cells[row, 9].Value = dh.DaThanhToan ? "Đã thanh toán" : "Chờ thanh toán";
                    worksheet.Cells[row, 10].Value = dh.VanChuyen?.TrangThaiGiaoHang.ToString() ?? "Chưa xử lý";

                    row++;
                }

                worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();

                var fileBytes = package.GetAsByteArray();

                return File(
                    fileBytes,
                    "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                    $"DanhSachDonHang_{DateTime.Now:yyyyMMddHHmmss}.xlsx"
                );
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Lỗi khi xuất Excel: " + ex.Message });
            }
        }


    }
}