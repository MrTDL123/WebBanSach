using Media.Models;
using Meida.DataAccess.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace ProjectCuoiKi.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class UserController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly ILogger<UserController> _logger;

        public UserController(ApplicationDbContext db, ILogger<UserController> logger)
        {
            _db = db;
            _logger = logger;
        }

        // 📚 DANH SÁCH NGƯỜI DÙNG
        public IActionResult QuanLyNguoiDung()
        {
            try
            {
                var dsKhachHang = _db.KhachHangs
                    .Include(k => k.TaiKhoan)
                    .OrderBy(k => k.HoTen)
                    .ToList();

                var dsNhanVien = _db.NhanViens
                    .Include(n => n.TaiKhoan)
                    .OrderBy(n => n.HoTen)
                    .ToList();

                ViewBag.KhachHangs = dsKhachHang;
                ViewBag.NhanViens = dsNhanVien;

                return View();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi tải danh sách người dùng");
                TempData["Error"] = "Lỗi khi tải dữ liệu người dùng";
                return View();
            }
        }

        // ➕ THÊM NHÂN VIÊN - GET
        [HttpGet]
        public IActionResult ThemNhanVien()
        {
            return View(new NhanVien
            {
                NgayVaoLam = DateTime.Today,
                NgaySinh = DateTime.Today.AddYears(-25)
            });
        }

        // ➕ THÊM NHÂN VIÊN - POST (DEBUG VERSION)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ThemNhanVien(NhanVien model)
        {
            try
            {
                Console.WriteLine("=== DEBUG START ===");
                Console.WriteLine($"Model received - HoTen: {model.HoTen}, NgayVaoLam: {model.NgayVaoLam}");

                // TẮT VALIDATION CHO TẤT CẢ NAVIGATION PROPERTIES
                ModelState.Remove("TaiKhoan");
                ModelState.Remove("DonHangs");
                ModelState.Remove("PhieuTraHangs");
                ModelState.Remove("ChamSocKhachHangs");
                ModelState.Remove("MaTaiKhoan");

                // DEBUG: Log tất cả ModelState errors
                foreach (var key in ModelState.Keys)
                {
                    var state = ModelState[key];
                    foreach (var error in state.Errors)
                    {
                        Console.WriteLine($"ModelState Error - Key: {key}, Error: {error.ErrorMessage}");
                    }
                }

                Console.WriteLine($"ModelState IsValid: {ModelState.IsValid}");
                Console.WriteLine("=== DEBUG END ===");

                // Kiểm tra validation thủ công
                if (string.IsNullOrEmpty(model.HoTen))
                {
                    TempData["Error"] = "Họ tên là bắt buộc";
                    return View(model);
                }

                if (model.NgayVaoLam == null)
                {
                    TempData["Error"] = "Ngày vào làm là bắt buộc";
                    return View(model);
                }

                // Kiểm tra CCCD đã tồn tại chưa
                if (!string.IsNullOrEmpty(model.CCCD) && await _db.NhanViens.AnyAsync(n => n.CCCD == model.CCCD))
                {
                    TempData["Error"] = "CCCD đã tồn tại trong hệ thống";
                    return View(model);
                }

                // Tạo username và email tự động
                var username = $"nv{DateTime.Now:yyyyMMddHHmmss}";
                var email = $"{username}@company.com";

                // Kiểm tra email đã tồn tại chưa
                if (await _db.Users.AnyAsync(u => u.Email == email))
                {
                    TempData["Error"] = "Email đã tồn tại, vui lòng thử lại";
                    return View(model);
                }

                // Tạo tài khoản mặc định
                var taiKhoan = new TaiKhoan
                {
                    UserName = username,
                    Email = email,
                    PhoneNumber = null,
                    EmailConfirmed = true,
                    LockoutEnabled = false,
                    SecurityStamp = Guid.NewGuid().ToString(),
                    ConcurrencyStamp = Guid.NewGuid().ToString(),
                    NormalizedUserName = username.ToUpper(),
                    NormalizedEmail = email.ToUpper()
                };

                Console.WriteLine($"Creating TaiKhoan: {taiKhoan.UserName}, Email: {taiKhoan.Email}");

                _db.Users.Add(taiKhoan);
                await _db.SaveChangesAsync();

                Console.WriteLine($"TaiKhoan created with ID: {taiKhoan.Id}");

                // Tạo nhân viên với MaTaiKhoan đã được tạo
                var nhanVien = new NhanVien
                {
                    MaTaiKhoan = taiKhoan.Id, // Gán MaTaiKhoan từ tài khoản vừa tạo
                    HoTen = model.HoTen?.Trim(),
                    DiaChi = model.DiaChi?.Trim(),
                    NgaySinh = model.NgaySinh,
                    CCCD = model.CCCD?.Trim(),
                    Luong = model.Luong,
                    BacLuong = model.BacLuong,
                    NgayVaoLam = model.NgayVaoLam ?? DateTime.Today,
                    QueQuan = model.QueQuan?.Trim()
                };

                Console.WriteLine($"Creating NhanVien: {nhanVien.HoTen}, MaTaiKhoan: {nhanVien.MaTaiKhoan}");

                _db.NhanViens.Add(nhanVien);
                await _db.SaveChangesAsync();

                Console.WriteLine("NhanVien created successfully");

                TempData["Success"] = $"Thêm nhân viên {nhanVien.HoTen} thành công!";
                return RedirectToAction(nameof(QuanLyNguoiDung));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi thêm nhân viên");
                Console.WriteLine($"EXCEPTION: {ex.Message}");
                Console.WriteLine($"INNER EXCEPTION: {ex.InnerException?.Message}");

                var errorMessage = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                TempData["Error"] = $"Có lỗi xảy ra khi thêm nhân viên: {errorMessage}";

                return View(model);
            }
        }

        // ✏ SỬA NHÂN VIÊN - GET
        [HttpGet]
        public IActionResult SuaNhanVien(int id)
        {
            try
            {
                var nhanVien = _db.NhanViens
                    .Include(n => n.TaiKhoan)
                    .FirstOrDefault(n => n.MaNhanVien == id);

                if (nhanVien == null)
                {
                    TempData["Error"] = "Không tìm thấy nhân viên";
                    return RedirectToAction(nameof(QuanLyNguoiDung));
                }

                return View(nhanVien);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi tải form sửa nhân viên");
                TempData["Error"] = "Lỗi khi tải thông tin nhân viên";
                return RedirectToAction(nameof(QuanLyNguoiDung));
            }
        }

        // ✏ SỬA NHÂN VIÊN - POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SuaNhanVien(NhanVien model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View(model);
                }

                var nhanVien = await _db.NhanViens
                    .Include(n => n.TaiKhoan)
                    .FirstOrDefaultAsync(n => n.MaNhanVien == model.MaNhanVien);

                if (nhanVien == null)
                {
                    TempData["Error"] = "Không tìm thấy nhân viên";
                    return RedirectToAction(nameof(QuanLyNguoiDung));
                }

                // Kiểm tra CCCD trùng (trừ chính nó)
                if (!string.IsNullOrEmpty(model.CCCD) &&
                    _db.NhanViens.Any(n => n.CCCD == model.CCCD && n.MaNhanVien != model.MaNhanVien))
                {
                    ModelState.AddModelError("CCCD", "CCCD đã tồn tại trong hệ thống");
                    return View(model);
                }

                // Cập nhật thông tin
                nhanVien.HoTen = model.HoTen.Trim();
                nhanVien.DiaChi = model.DiaChi?.Trim();
                nhanVien.NgaySinh = model.NgaySinh;
                nhanVien.CCCD = model.CCCD?.Trim();
                nhanVien.Luong = model.Luong;
                nhanVien.BacLuong = model.BacLuong;
                nhanVien.NgayVaoLam = model.NgayVaoLam;
                nhanVien.QueQuan = model.QueQuan?.Trim();

                _db.NhanViens.Update(nhanVien);
                await _db.SaveChangesAsync();

                TempData["Success"] = $"Cập nhật thông tin nhân viên {nhanVien.HoTen} thành công!";
                return RedirectToAction(nameof(QuanLyNguoiDung));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi cập nhật nhân viên");
                ModelState.AddModelError("", "Có lỗi xảy ra khi cập nhật thông tin. Vui lòng thử lại.");
                return View(model);
            }
        }

        // 👁 CHI TIẾT KHÁCH HÀNG
        public IActionResult ChiTietKhachHang(int id)
        {
            try
            {
                var khachHang = _db.KhachHangs
     .Include(k => k.TaiKhoan)
     .Include(k => k.DonHangs)
         .ThenInclude(d => d.ChiTietDonHangs) // Lấy chi tiết sản phẩm của từng đơn  
     .Include(k => k.PhanHoiKhachHangs)
     .Include(k => k.DanhGiaSanPhams)
     .Include(k => k.DiaChiNhanHangs)
     .FirstOrDefault(k => k.MaKhachHang == id);


                if (khachHang == null)
                {
                    TempData["Error"] = "Không tìm thấy khách hàng";
                    return RedirectToAction(nameof(QuanLyNguoiDung));
                }

                ViewBag.TongDonHang = khachHang.DonHangs?.Count ?? 0;
                ViewBag.TongPhanHoi = khachHang.PhanHoiKhachHangs?.Count ?? 0;
                ViewBag.TongDanhGia = khachHang.DanhGiaSanPhams?.Count ?? 0;
                ViewBag.TongDiaChi = khachHang.DiaChiNhanHangs?.Count ?? 0;

                return View(khachHang);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi xem chi tiết khách hàng");
                TempData["Error"] = "Lỗi khi tải thông tin khách hàng";
                return RedirectToAction(nameof(QuanLyNguoiDung));
            }
        }

        // 👁 CHI TIẾT NHÂN VIÊN
        public IActionResult ChiTietNhanVien(int id)
        {
            try
            {
                var nhanVien = _db.NhanViens
                    .Include(n => n.TaiKhoan)
                    .Include(n => n.DonHangs)
                    .Include(n => n.PhieuTraHangs)
                    .Include(n => n.ChamSocKhachHangs)
                    .FirstOrDefault(n => n.MaNhanVien == id);

                if (nhanVien == null)
                {
                    TempData["Error"] = "Không tìm thấy nhân viên";
                    return RedirectToAction(nameof(QuanLyNguoiDung));
                }

                ViewBag.TongDonHang = nhanVien.DonHangs?.Count ?? 0;
                ViewBag.TongPhieuTra = nhanVien.PhieuTraHangs?.Count ?? 0;
                ViewBag.TongChamSoc = nhanVien.ChamSocKhachHangs?.Count ?? 0;

                return View(nhanVien);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi xem chi tiết nhân viên");
                TempData["Error"] = "Lỗi khi tải thông tin nhân viên";
                return RedirectToAction(nameof(QuanLyNguoiDung));
            }
        }

        // ❌ XÓA KHÁCH HÀNG
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> XoaKhachHang(int id)
        {
            try
            {
                var khachHang = await _db.KhachHangs
                    .Include(k => k.TaiKhoan)
                    .Include(k => k.DonHangs)
                    .Include(k => k.PhanHoiKhachHangs)
                    .Include(k => k.ChamSocKhachHangs)
                    .Include(k => k.DiaChiNhanHangs)
                    .Include(k => k.DanhGiaSanPhams)
                    .Include(k => k.LuotThichDanhGiaSanPhams)
                    .Include(k => k.GioHang)
                    .FirstOrDefaultAsync(k => k.MaKhachHang == id);

                if (khachHang == null)
                {
                    TempData["Error"] = "Không tìm thấy khách hàng";
                    return RedirectToAction(nameof(QuanLyNguoiDung));
                }

                // Kiểm tra ràng buộc
                if (khachHang.DonHangs?.Any() == true)
                {
                    TempData["Error"] = $"Không thể xóa khách hàng '{khachHang.HoTen}' vì có {khachHang.DonHangs.Count} đơn hàng liên quan";
                    return RedirectToAction(nameof(QuanLyNguoiDung));
                }

                // Xóa dữ liệu liên quan
                if (khachHang.PhanHoiKhachHangs?.Any() == true)
                {
                    _db.PhanHoiKhachHangs.RemoveRange(khachHang.PhanHoiKhachHangs);
                }

                if (khachHang.ChamSocKhachHangs?.Any() == true)
                {
                    _db.ChamSocKhachHangs.RemoveRange(khachHang.ChamSocKhachHangs);
                }

                if (khachHang.DiaChiNhanHangs?.Any() == true)
                {
                    _db.DiaChiNhanHangs.RemoveRange(khachHang.DiaChiNhanHangs);
                }

                if (khachHang.DanhGiaSanPhams?.Any() == true)
                {
                    _db.DanhGiaSanPhams.RemoveRange(khachHang.DanhGiaSanPhams);
                }

                if (khachHang.LuotThichDanhGiaSanPhams?.Any() == true)
                {
                    _db.LuotThichDanhGiaSanPhams.RemoveRange(khachHang.LuotThichDanhGiaSanPhams);
                }

                if (khachHang.GioHang != null)
                {
                    _db.GioHangs.Remove(khachHang.GioHang);
                }

                // Xóa khách hàng và tài khoản
                _db.KhachHangs.Remove(khachHang);

                if (khachHang.TaiKhoan != null)
                {
                    _db.Users.Remove(khachHang.TaiKhoan);
                }

                await _db.SaveChangesAsync();

                TempData["Success"] = $"Đã xóa khách hàng {khachHang.HoTen} thành công!";
                return RedirectToAction(nameof(QuanLyNguoiDung));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi xóa khách hàng");
                TempData["Error"] = "Có lỗi xảy ra khi xóa khách hàng. Vui lòng thử lại.";
                return RedirectToAction(nameof(QuanLyNguoiDung));
            }
        }

        // ❌ XÓA NHÂN VIÊN
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> XoaNhanVien(int id)
        {
            try
            {
                var nhanVien = await _db.NhanViens
                    .Include(n => n.TaiKhoan)
                    .Include(n => n.DonHangs)
                    .Include(n => n.PhieuTraHangs)
                    .Include(n => n.ChamSocKhachHangs)
                    .FirstOrDefaultAsync(n => n.MaNhanVien == id);

                if (nhanVien == null)
                {
                    TempData["Error"] = "Không tìm thấy nhân viên";
                    return RedirectToAction(nameof(QuanLyNguoiDung));
                }

                // Kiểm tra ràng buộc
                if (nhanVien.DonHangs?.Any() == true || nhanVien.PhieuTraHangs?.Any() == true)
                {
                    var donHangCount = nhanVien.DonHangs?.Count ?? 0;
                    var phieuTraCount = nhanVien.PhieuTraHangs?.Count ?? 0;
                    TempData["Error"] = $"Không thể xóa nhân viên '{nhanVien.HoTen}' vì có {donHangCount} đơn hàng và {phieuTraCount} phiếu trả liên quan";
                    return RedirectToAction(nameof(QuanLyNguoiDung));
                }

                // Xóa dữ liệu liên quan
                if (nhanVien.ChamSocKhachHangs?.Any() == true)
                {
                    _db.ChamSocKhachHangs.RemoveRange(nhanVien.ChamSocKhachHangs);
                }

                // Xóa nhân viên và tài khoản
                _db.NhanViens.Remove(nhanVien);

                if (nhanVien.TaiKhoan != null)
                {
                    _db.Users.Remove(nhanVien.TaiKhoan);
                }

                await _db.SaveChangesAsync();

                TempData["Success"] = $"Đã xóa nhân viên {nhanVien.HoTen} thành công!";
                return RedirectToAction(nameof(QuanLyNguoiDung));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi xóa nhân viên");
                TempData["Error"] = "Có lỗi xảy ra khi xóa nhân viên. Vui lòng thử lại.";
                return RedirectToAction(nameof(QuanLyNguoiDung));
            }
        }

        // 🔄 CẬP NHẬT TRẠNG THÁI NHÂN VIÊN
        [HttpPost]
        public async Task<IActionResult> CapNhatTrangThaiNhanVien(int id, bool trangThai)
        {
            try
            {
                var nhanVien = await _db.NhanViens.FindAsync(id);
                if (nhanVien == null)
                {
                    return Json(new { success = false, message = "Không tìm thấy nhân viên" });
                }

                // Nếu model có property TrangThai, bạn có thể thêm vào
                // nhanVien.TrangThai = trangThai;

                _db.NhanViens.Update(nhanVien);
                await _db.SaveChangesAsync();

                return Json(new { success = true, message = "Cập nhật trạng thái thành công" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi cập nhật trạng thái nhân viên");
                return Json(new { success = false, message = "Lỗi khi cập nhật trạng thái" });
            }
        }
    }
}