using Media.Areas.Admin.Controllers;
using Media.Models;
using Media.Models.ViewModels;
using Media.Utility;
using Meida.DataAccess.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace ProjectCuoiKi.Areas.Admin.Controllers
{
    public class UserController : AdminController
    {
        private readonly ApplicationDbContext _db;
        private readonly ILogger<UserController> _logger;
        private readonly SignInManager<TaiKhoan> _signInManager;
        private readonly UserManager<TaiKhoan> _userManager;

        public UserController(ApplicationDbContext db, ILogger<UserController> logger, SignInManager<TaiKhoan> signInManager, UserManager<TaiKhoan> userManager)
        {
            _db = db;
            _logger = logger;
            _signInManager = signInManager;
            _userManager = userManager;
        }

        #region Đăng Nhập
        [HttpGet]
        [AllowAnonymous]
        public IActionResult AccessDenied()
        {
            return View();
        }
        [AllowAnonymous]
        [HttpGet]
        public IActionResult DangNhap(string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DangNhap(DangNhapAdminVM model, string returnUrl = null)
        {
            if (!ModelState.IsValid) return View(model);

            var userNameToSignIn = model.UserName;

            if(model.UserName.Contains("@")) 
            {
                TaiKhoan? userByEmail = await _userManager.FindByEmailAsync(model.UserName);
                if (userByEmail != null)
                {
                    userNameToSignIn = userByEmail.UserName;
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Email sai hoặc không tồn tại.");
                    return View(model);
                }
            }

            var result = await _signInManager.PasswordSignInAsync(
                userNameToSignIn,
                model.MatKhau,
                model.GhiNhoDangNhap,
                lockoutOnFailure: false
            );

            if (result.Succeeded)
            {
                TaiKhoan user = await _userManager.FindByNameAsync(userNameToSignIn);
                if(await _userManager.IsInRoleAsync(user, SD.Role_Admin) ||
                   await _userManager.IsInRoleAsync(user, SD.Role_Employee))
                {

                    if(!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                    {
                        return Redirect(returnUrl);
                    }
                    return RedirectToAction("Index", "DashBoard", new { area = "Admin" });
                }

                await _signInManager.SignOutAsync();
                ModelState.AddModelError(string.Empty, "Tài khoản không có quyền hạn");
                return View(model);
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Tên đăng nhập/Email hoặc mật khẩu không đúng.");
                return View(model);
            }
        }

        #endregion

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

        [HttpGet]
        public IActionResult ThemNhanVien()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ThemNhanVien(ThemNhanVienVM model)
        {
            try
            {
                Console.WriteLine("=== DEBUG START ===");
                Console.WriteLine($"Model received - HoTen: {model.NhanVienMoi.HoTen}, NgayVaoLam: {model.NhanVienMoi.NgayVaoLam}");

                ModelState.Remove("TaiKhoan");
                ModelState.Remove("DonHangs");
                ModelState.Remove("PhieuTraHangs");
                ModelState.Remove("ChamSocKhachHangs");
                ModelState.Remove("MaTaiKhoan");

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

                if (string.IsNullOrEmpty(model.NhanVienMoi.HoTen))
                {
                    TempData["Error"] = "Họ tên là bắt buộc";
                    return View(model);
                }

                if (model.NhanVienMoi.NgayVaoLam == null)
                {
                    TempData["Error"] = "Ngày vào làm là bắt buộc";
                    return View(model);
                }

                if (!string.IsNullOrEmpty(model.NhanVienMoi.CCCD) && await _db.NhanViens.AnyAsync(n => n.CCCD == model.NhanVienMoi.CCCD))
                {
                    TempData["Error"] = "CCCD đã tồn tại trong hệ thống";
                    return View(model);
                }

                var existingEmail = await _userManager.FindByEmailAsync(model.NhanVienMoi.Email);
                if (existingEmail != null)
                {
                    TempData["Error"] = "Email này đã được đăng ký.";
                    return View(model);
                }

                var existingPhone = await _userManager.Users.FirstOrDefaultAsync(u => u.PhoneNumber == model.NhanVienMoi.DienThoai);
                if (existingPhone != null)
                {
                    TempData["Error"] = "Số điện thoại này đã được sử dụng.";
                    return View(model);
                }


                var username = model.NhanVienMoi.Email;
                var email = model.NhanVienMoi.Email;

                var taiKhoan = new TaiKhoan
                {
                    UserName = username,
                    Email = email,
                    PhoneNumber = model.NhanVienMoi.DienThoai,
                    EmailConfirmed = true
                };

                var result = await _userManager.CreateAsync(taiKhoan, model.MatKhau);
                if (result.Succeeded)
                {
                    var nhanVienMoi = new NhanVien
                    {
                        MaTaiKhoan = taiKhoan.Id,
                        HoTen = model.NhanVienMoi.HoTen,
                        Email = model.NhanVienMoi.Email,
                        DienThoai = model.NhanVienMoi.DienThoai,
                        DiaChi = model.NhanVienMoi.DiaChi?.Trim(),
                        NgaySinh = model.NhanVienMoi.NgaySinh,
                        CCCD = model.NhanVienMoi.CCCD?.Trim(),
                        Luong = model.NhanVienMoi.Luong,
                        BacLuong = model.NhanVienMoi.BacLuong,
                        NgayVaoLam = model.NhanVienMoi.NgayVaoLam ?? DateTime.Today,
                        QueQuan = model.NhanVienMoi.QueQuan?.Trim()
                    };

                    _db.NhanViens.Add(nhanVienMoi);
                    await _db.SaveChangesAsync();
                    await _userManager.AddClaimAsync(taiKhoan, new Claim("HoTen", nhanVienMoi.HoTen));

                    await _userManager.AddToRoleAsync(taiKhoan, SD.Role_Employee);
                }
                TempData["Success"] = $"Thêm nhân viên thành công!";
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SuaNhanVien(NhanVien model)
        {
            try
            {
                ModelState.Remove("TaiKhoan");
                ModelState.Remove("TaiKhoanId"); 
                ModelState.Remove("DonHangs");   
                ModelState.Remove("PhieuTraHangs");
                ModelState.Remove("ChamSocKhachHangs");
                ModelState.Remove("Email");
                ModelState.Remove("DienThoai");

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

                if (!string.IsNullOrEmpty(model.CCCD) &&
                    _db.NhanViens.Any(n => n.CCCD == model.CCCD && n.MaNhanVien != model.MaNhanVien))
                {
                    ModelState.AddModelError("CCCD", "CCCD đã tồn tại trong hệ thống");
                    return View(model);
                }

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