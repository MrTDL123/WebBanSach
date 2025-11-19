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

        public UserController(ApplicationDbContext db)
        {
            _db = db;
        }

        // 📚 Danh sách người dùng
        public IActionResult QuanLyNguoiDung()
        {
            var dsKhachHang = _db.KhachHangs
                .Include(k => k.TaiKhoan)
                .ToList();

            var dsNhanVien = _db.NhanViens
                .Include(n => n.TaiKhoan)
                .ToList();

            ViewBag.KhachHangs = dsKhachHang;
            ViewBag.NhanViens = dsNhanVien;

            return View();
        }

        // ❌ XÓA action Thêm khách hàng - KHÔNG CHO PHÉP THÊM KHÁCH HÀNG

        // ➕ GET: Thêm nhân viên
        [HttpGet]
        public IActionResult ThemNhanVien()
        {
            return View(new NhanVien());
        }

        // ➕ POST: Thêm nhân viên
        [HttpPost]
        public IActionResult ThemNhanVien(NhanVien model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // Kiểm tra email đã tồn tại chưa
            if (_db.Users.Any(u => u.Email == model.TaiKhoan.Email))
            {
                ModelState.AddModelError("TaiKhoan.Email", "Email đã tồn tại trong hệ thống");
                return View(model);
            }

            try
            {
                // Tạo tài khoản Identity
                var user = new TaiKhoan
                {
                    UserName = model.TaiKhoan.Email,
                    Email = model.TaiKhoan.Email,
                    PhoneNumber = model.TaiKhoan.PhoneNumber,
                    EmailConfirmed = true
                };

                _db.Users.Add(user);
                _db.SaveChanges();

                // Tạo nhân viên
                var nhanVien = new NhanVien
                {
                    MaTaiKhoan = user.Id,
                    HoTen = model.HoTen,
                    DiaChi = model.DiaChi,
                    NgaySinh = model.NgaySinh,
                    CCCD = model.CCCD,
                    Luong = model.Luong,
                    BacLuong = model.BacLuong,
                    NgayVaoLam = model.NgayVaoLam ?? DateTime.Now,
                    QueQuan = model.QueQuan
                };

                _db.NhanViens.Add(nhanVien);
                _db.SaveChanges();

                TempData["Success"] = "Thêm nhân viên thành công!";
                return RedirectToAction("QuanLyNguoiDung");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Có lỗi xảy ra khi thêm nhân viên: " + ex.Message);
                return View(model);
            }
        }

        // ✏ GET: Sửa thông tin khách hàng - CHỈ CHO XEM, KHÔNG CHO SỬA
        [HttpGet]
        public IActionResult SuaThongTin(int id)
        {
            var khachHang = _db.KhachHangs
                .Include(k => k.TaiKhoan)
                .FirstOrDefault(k => k.MaKhachHang == id);

            if (khachHang == null) return NotFound();

            // Chỉ trả về view xem, không cho phép chỉnh sửa
            ViewBag.IsReadOnly = true;
            return View("ChiTietKhachHang", khachHang); // Chuyển hướng đến view chi tiết
        }

        // ❌ XÓA action POST Sửa thông tin khách hàng - KHÔNG CHO PHÉP SỬA

        // ✏ GET: Sửa thông tin nhân viên
        [HttpGet]
        public IActionResult SuaNhanVien(int id)
        {
            var nhanVien = _db.NhanViens
                .Include(n => n.TaiKhoan)
                .FirstOrDefault(n => n.MaNhanVien == id);

            if (nhanVien == null) return NotFound();

            return View(nhanVien);
        }

        // ✏ POST: Sửa thông tin nhân viên
        [HttpPost]
        public IActionResult SuaNhanVien(NhanVien model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var nhanVien = _db.NhanViens
                .Include(n => n.TaiKhoan)
                .FirstOrDefault(n => n.MaNhanVien == model.MaNhanVien);

            if (nhanVien == null) return NotFound();

            try
            {
                // Cập nhật thông tin nhân viên
                nhanVien.HoTen = model.HoTen;
                nhanVien.DiaChi = model.DiaChi;
                nhanVien.NgaySinh = model.NgaySinh;
                nhanVien.CCCD = model.CCCD;
                nhanVien.Luong = model.Luong;
                nhanVien.BacLuong = model.BacLuong;
                nhanVien.NgayVaoLam = model.NgayVaoLam;
                nhanVien.QueQuan = model.QueQuan;

                _db.NhanViens.Update(nhanVien);
                _db.SaveChanges();

                TempData["Success"] = "Cập nhật thông tin nhân viên thành công!";
                return RedirectToAction("QuanLyNguoiDung");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Có lỗi xảy ra khi cập nhật: " + ex.Message);
                return View(model);
            }
        }

        // ❌ Xóa khách hàng
        public IActionResult XoaUser(int id)
        {
            var khachHang = _db.KhachHangs
                .Include(k => k.TaiKhoan)
                .Include(k => k.DonHangs)
                .Include(k => k.PhanHoiKhachHangs)
                .Include(k => k.ChamSocKhachHangs)
                .Include(k => k.DiaChiNhanHangs)
                .FirstOrDefault(k => k.MaKhachHang == id);

            if (khachHang == null) return NotFound();

            // Kiểm tra xem khách hàng có đơn hàng không
            if (khachHang.DonHangs?.Any() == true)
            {
                TempData["Error"] = $"Không thể xóa khách hàng '{khachHang.HoTen}' vì có đơn hàng liên quan!";
                return RedirectToAction("QuanLyNguoiDung");
            }

            try
            {
                // Xóa các dữ liệu liên quan
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

                // Xóa giỏ hàng nếu có
                var gioHang = _db.GioHangs.FirstOrDefault(g => g.MaKhachHang == id);
                if (gioHang != null)
                {
                    _db.GioHangs.Remove(gioHang);
                }

                // Xóa khách hàng
                _db.KhachHangs.Remove(khachHang);

                // Xóa tài khoản Identity
                if (khachHang.TaiKhoan != null)
                {
                    _db.Users.Remove(khachHang.TaiKhoan);
                }

                _db.SaveChanges();

                TempData["Success"] = "Xóa khách hàng thành công!";
                return RedirectToAction("QuanLyNguoiDung");
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Có lỗi xảy ra khi xóa khách hàng: {ex.Message}";
                return RedirectToAction("QuanLyNguoiDung");
            }
        }

        // ❌ Xóa nhân viên
        public IActionResult XoaNhanVien(int id)
        {
            var nhanVien = _db.NhanViens
                .Include(n => n.TaiKhoan)
                .Include(n => n.DonHangs)
                .Include(n => n.PhieuTraHangs)
                .Include(n => n.ChamSocKhachHangs)
                .FirstOrDefault(n => n.MaNhanVien == id);

            if (nhanVien == null) return NotFound();

            // Kiểm tra xem nhân viên có dữ liệu liên quan không
            if (nhanVien.DonHangs?.Any() == true || nhanVien.PhieuTraHangs?.Any() == true)
            {
                TempData["Error"] = $"Không thể xóa nhân viên '{nhanVien.HoTen}' vì có dữ liệu liên quan!";
                return RedirectToAction("QuanLyNguoiDung");
            }

            try
            {
                // Xóa dữ liệu chăm sóc khách hàng liên quan
                if (nhanVien.ChamSocKhachHangs?.Any() == true)
                {
                    _db.ChamSocKhachHangs.RemoveRange(nhanVien.ChamSocKhachHangs);
                }

                // Xóa nhân viên
                _db.NhanViens.Remove(nhanVien);

                // Xóa tài khoản Identity
                if (nhanVien.TaiKhoan != null)
                {
                    _db.Users.Remove(nhanVien.TaiKhoan);
                }

                _db.SaveChanges();

                TempData["Success"] = "Xóa nhân viên thành công!";
                return RedirectToAction("QuanLyNguoiDung");
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Có lỗi xảy ra khi xóa nhân viên: {ex.Message}";
                return RedirectToAction("QuanLyNguoiDung");
            }
        }

        // 👁 Chi tiết khách hàng - CHỈ XEM
        public IActionResult ChiTietKhachHang(int id)
        {
            var khachHang = _db.KhachHangs
                .Include(k => k.TaiKhoan)
                .Include(k => k.DonHangs)
                .Include(k => k.PhanHoiKhachHangs)
                .Include(k => k.DiaChiNhanHangs)
                .FirstOrDefault(k => k.MaKhachHang == id);

            if (khachHang == null) return NotFound();

            ViewBag.IsReadOnly = true; // Đánh dấu là chỉ xem
            return View(khachHang);
        }

        // 👁 Chi tiết nhân viên
        public IActionResult ChiTietNhanVien(int id)
        {
            var nhanVien = _db.NhanViens
                .Include(n => n.TaiKhoan)
                .Include(n => n.DonHangs)
                .Include(n => n.PhieuTraHangs)
                .FirstOrDefault(n => n.MaNhanVien == id);

            if (nhanVien == null) return NotFound();

            return View(nhanVien);
        }
    }
}