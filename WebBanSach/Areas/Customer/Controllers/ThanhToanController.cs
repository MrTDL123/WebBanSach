using Media.DataAccess.Repository.IRepository;
using Media.Models;
using Media.Models.ViewModels;
using Media.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Runtime.InteropServices;
using System.Security.Claims;
using Media.Utility;

namespace WebBanSach.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class ThanhToanController : Controller
    {
        private readonly IUnitOfWork _unit;
        private readonly LocationService _locationService;
        private readonly IGioHangService _gioHangService;
        public ThanhToanController(IUnitOfWork unit, LocationService locationService, IGioHangService gioHangService)
        {
            _unit = unit;
            _locationService = locationService;
            _gioHangService = gioHangService;
        }

        [HttpPost]
        public IActionResult LayDanhSachSanPhamThanhToan([FromBody] DanhSachSanPhamGioHangRequest request)
        {
            // Khai báo biến
            List<GioHangVM> gioHangHienTai = new List<GioHangVM>();

            try
            {
                if (User.Identity.IsAuthenticated)
                {
                    var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                    gioHangHienTai = _gioHangService.TaiGioHangTuDb(userId);
                }
                else
                {
                    gioHangHienTai = HttpContext.Session.GetObjectFromJson<List<GioHangVM>>("GioHang") ?? new List<GioHangVM>();
                }

                if (request.DanhSachMaSach == null || !request.DanhSachMaSach.Any())
                    return Json(new { success = false, message = "Không có sản phẩm nào được chọn." });

                if (!gioHangHienTai.Any())
                    return Json(new { success = false, message = "Giỏ hàng của bạn đang trống hoặc phiên làm việc đã hết hạn." });

                var gioHangThanhToan = gioHangHienTai
                    .Where(sp => request.DanhSachMaSach.Contains(sp.MaSach))
                    .ToList();

                if (!gioHangThanhToan.Any())
                    return Json(new { success = false, message = "Không tìm thấy sản phẩm được chọn." });

                HttpContext.Session.SetObjectAsJson("GioHangThanhToan", gioHangThanhToan);

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Lỗi xử lý dữ liệu: {ex.Message}" });
            }
        }

        public IActionResult ChonDiaChiThanhToan()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var khachHang = _unit.KhachHangs.Get(kh => kh.MaTaiKhoan == userId);

            var danhSachDiaChi = _unit.DiaChiNhanHangs
                .GetRange(dc => dc.MaKhachHang == khachHang.MaKhachHang && dc.LaDaXoa == false)
                .OrderByDescending(dc => dc.LaMacDinh)
                .ToList();

            return PartialView("_ChonDiaChiThanhToanPartial", danhSachDiaChi);
        }

        [HttpGet]
        public async Task<IActionResult> ThanhToan()
        {
            if (!User.Identity.IsAuthenticated)
            {
                TempData["ReturnUrl"] = Url.Action("ThanhToan", "ThanhToan", new { area = "Customer" });
                return RedirectToAction("DangNhap", "KhachHang", new { area = "Customer" });
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var khachHang = _unit.KhachHangs.Get(kh => kh.MaTaiKhoan == userId);

            var gioHangThanhToan = HttpContext.Session.GetObjectFromJson<List<GioHangVM>>("GioHangThanhToan");
            if (gioHangThanhToan == null || !gioHangThanhToan.Any())
            {
                return RedirectToAction("TrangChu", "Home", new { area = "Customer" });
            }

            // ⭐ BẮT ĐẦU LOGIC MỚI
            // 1. Tải tất cả địa chỉ đã lưu
            var danhSachDiaChi = _unit.DiaChiNhanHangs
                .GetRange(dc => dc.MaKhachHang == khachHang.MaKhachHang && dc.LaDaXoa == false)
                .OrderByDescending(dc => dc.LaMacDinh)
                .ToList();

            // 2. Tìm địa chỉ mặc định (hoặc cái đầu tiên nếu không có)
            var diaChiMacDinh = danhSachDiaChi.FirstOrDefault(d => d.LaMacDinh) ?? danhSachDiaChi.FirstOrDefault();

            var model = new ThanhToan
            {
                DanhSachSanPham = gioHangThanhToan,
                DanhSachDiaChi = danhSachDiaChi,
                DiaChiMacDinh = diaChiMacDinh,
                DanhSachTinhThanh = await _locationService.GetProvincesAsync(),
                DanhSachQuanHuyen = new List<SelectListItem>(),
                DanhSachPhuongXa = new List<SelectListItem>(),
                TamTinh = gioHangThanhToan.Sum(sp => sp.ThanhTien),
                PhiVanChuyen = 20000,
                TongTien = gioHangThanhToan.Sum(sp => sp.ThanhTien) + 20000

            };

            if (diaChiMacDinh != null)
            {
                model.HoTen = diaChiMacDinh.TenNguoiNhan;
                model.SoDienThoai = diaChiMacDinh.SoDienThoai;
                model.DiaChi = diaChiMacDinh.DiaChiChiTiet;
                model.TinhThanh = diaChiMacDinh.TinhThanh; // Gán TÊN
                model.QuanHuyen = diaChiMacDinh.QuanHuyen; // Gán TÊN
                model.PhuongXa = diaChiMacDinh.PhuongXa;   // Gán TÊN
                model.MaDiaChiNhanHang = diaChiMacDinh.MaDiaChi; // Gán MÃ
            }

            return View(model);
        }

        [HttpPost]
        public IActionResult CapNhatThanhTienKhiThanhToan(int maSach, int soLuong)
        {
            // ⚙️ Giả sử bạn lấy giỏ hàng từ session
            var gioHang = HttpContext.Session.GetObjectFromJson<List<GioHangVM>>("GioHangThanhToan");
            var sanPham = gioHang.FirstOrDefault(x => x.MaSach == maSach);
            if (sanPham == null)
                return Json(new { success = false });

            // ✅ Cập nhật số lượng
            sanPham.SoLuong = soLuong;

            // ✅ Tính lại tổng phụ & tổng tiền
            var tamTinh = gioHang.Sum(x => x.ThanhTien);
            var phiVanChuyen = 20000;
            var tongTien = tamTinh + phiVanChuyen;

            // ✅ Lưu lại session
            HttpContext.Session.SetObjectAsJson("GioHangThanhToan", gioHang);

            return Json(new
            {
                success = true,
                thanhTien = sanPham.ThanhTien,
                thanhTienFormatted = sanPham.ThanhTien.ToString("N0"),
                tamTinh = tamTinh,
                tamTinhFormatted = tamTinh.ToString("N0"),
                tongTien = tongTien,
                tongTienFormatted = tongTien.ToString("N0")
            });
        }

        [HttpPost]
        public IActionResult XoaKhoiThanhToan(int maSach)
        {
            try
            {
                // Giả sử danh sách sản phẩm được lưu trong session
                var gioHang = HttpContext.Session.GetObjectFromJson<List<GioHangVM>>("GioHangThanhToan") ?? new();

                var sanPham = gioHang.FirstOrDefault(sp => sp.MaSach == maSach);
                if (sanPham != null)
                {
                    gioHang.Remove(sanPham);
                    HttpContext.Session.SetObjectAsJson("GioHangThanhToan", gioHang);
                }

                // Tính lại tổng tiền
                decimal tamTinh = gioHang.Sum(sp => sp.ThanhTien);
                decimal phiVanChuyen = 20000; // giả định
                decimal tongTien = tamTinh + phiVanChuyen;

                return Json(new
                {
                    success = true,
                    tamTinhFormatted = tamTinh.ToString("N0"),
                    tongTienFormatted = tongTien.ToString("N0")
                });
            }
            catch
            {
                return Json(new { success = false });
            }
        }

        [HttpPost]
        public IActionResult LuuThongTinMua([FromBody] ThongTinMuaHangVM thongTin)
        {
            try
            {
                Sach sach = _unit.Saches.Get(s => s.MaSach == thongTin.MaSach);
                if (sach == null)
                {
                    return BadRequest("Sản phẩm không tồn tại.");
                }

                // 2. ✅ Tạo danh sách thanh toán chỉ với 1 sản phẩm này
                var gioHangThanhToan = new List<GioHangVM>
                {
                    new GioHangVM
                    {
                        MaSach = sach.MaSach,
                        TenSach = sach.TenSach,
                        GiaBan = sach.GiaBan,
                        GiaSauGiam = sach.GiaSauGiam,
                        AnhBiaChinh = sach.AnhBiaChinh,
                        SoLuong = thongTin.SoLuong
                    }
                };

                // 3. ✅ Sử dụng chung 1 session key
                // Giống hệt session key mà action LayDanhSachSanPhamThanhToan đang dùng
                HttpContext.Session.SetObjectAsJson("GioHangThanhToan", gioHangThanhToan);

                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest($"Dữ liệu không hợp lệ: {ex.Message}");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ThanhToan(ThanhToan model)
        {
            // 1. Lấy thông tin Khách Hàng
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var khachHang = _unit.KhachHangs.Get(kh => kh.MaTaiKhoan == userId);

            // Kiểm tra lỡ cookie còn nhưng tài khoản bị xóa
            if (khachHang == null)
            {
                return RedirectToAction("DangNhap", "KhachHang");
            }

            // 2. Lấy giỏ hàng đang chờ thanh toán từ Session
            var gioHangThanhToan = HttpContext.Session.GetObjectFromJson<List<GioHangVM>>("GioHangThanhToan");

            // Kiểm tra lỡ Session hết hạn (rất quan trọng!)
            if (gioHangThanhToan == null || !gioHangThanhToan.Any())
            {
                TempData["ErrorMessage"] = "Giỏ hàng của bạn đã hết hạn hoặc không tồn tại. Vui lòng thử lại.";
                return RedirectToAction("TrangChu"); // Hoặc trang Giỏ Hàng
            }

            // Gán lại DanhSachSanPham cho model (vì nó sẽ bị null khi POST)
            // Việc này để nếu ModelState không hợp lệ, View vẫn có dữ liệu sản phẩm
            model.DanhSachSanPham = gioHangThanhToan;
            model.TamTinh = gioHangThanhToan.Sum(sp => sp.ThanhTien);
            model.PhiVanChuyen = 20000;
            model.TongTien = model.TamTinh + model.PhiVanChuyen;

            bool kiemTraTonKhoThanhCong = true;
            foreach (var item in gioHangThanhToan)
            {
                // Lấy sản phẩm từ Database
                var sanPhamTrongKho = _unit.Saches.GetById(item.MaSach);

                if (sanPhamTrongKho == null)
                {
                    // Trường hợp sản phẩm bị xóa khỏi DB trong khi khách đang mua
                    ModelState.AddModelError(string.Empty, $"Sản phẩm '{item.TenSach}' không còn tồn tại.");
                    kiemTraTonKhoThanhCong = false;
                }
                else if (sanPhamTrongKho.SoLuong < item.SoLuong)
                {
                    // Trường hợp không đủ hàng
                    ModelState.AddModelError(string.Empty, $"Sản phẩm '{item.TenSach}' không đủ tồn kho (chỉ còn {sanPhamTrongKho.SoLuong} sản phẩm).");
                    kiemTraTonKhoThanhCong = false;
                }
            }

            // === BƯỚC 2: KIỂM TRA DỮ LIỆU FORM (MODELSTATE) ===
            if (!ModelState.IsValid || !kiemTraTonKhoThanhCong)
            {
                TempData["ErrorMessage"] = "Thông tin không hợp lệ, vui lòng kiểm tra lại.";

                // Nếu validation thất bại, ta phải tải lại TOÀN BỘ dữ liệu mà View cần
                model.DanhSachTinhThanh = await _locationService.GetProvincesAsync();
                model.DanhSachQuanHuyen = new List<SelectListItem>(); // Sẽ được JS tải lại
                model.DanhSachPhuongXa = new List<SelectListItem>(); // Sẽ được JS tải lại

                var diaChiList = _unit.DiaChiNhanHangs.GetRange(dc => dc.MaKhachHang == khachHang.MaKhachHang).ToList();
                model.DanhSachDiaChi = diaChiList;
                model.DiaChiMacDinh = diaChiList.FirstOrDefault(d => d.LaMacDinh) ?? diaChiList.FirstOrDefault();

                return View(model); // Trả về View với các lỗi validation
            }

            // === BƯỚC 3: TẠO DỮ LIỆU ĐƠN HÀNG (MỌI THỨ HỢP LỆ) ===

            // 3.1. Tạo Đơn Hàng (Bảng DonHang)
            var donHang = new DonHang
            {
                MaKhachHang = khachHang.MaKhachHang,
                MaNhanVien = null,
                MaDiaChi = model.MaDiaChiNhanHang,
                TenNguoiNhan = model.HoTen,
                SoDienThoaiNhan = model.SoDienThoai,
                TinhThanh = model.TinhThanh,
                QuanHuyen = model.QuanHuyen,
                PhuongXa = model.PhuongXa,
                DiaChiChiTiet = model.DiaChi,
                GhiChu = model.GhiChu,
                Total = model.TongTien,
                NgayTao = DateTime.Now,
                DaThanhToan = false,
                HinhThucThanhToan = model.HinhThucThanhToanChon,

                // ⭐ SỬA LẠI:
                // 5.2. Tạo Chi Tiết Đơn Hàng (Bảng ChiTietDonHang)
                ChiTietDonHangs = new List<ChiTietDonHang>(), // Khởi tạo list rỗng

                // 5.3. Tạo Hóa Đơn (Bảng HoaDon)
                HoaDon = new HoaDon
                {
                    NgayLap = DateTime.UtcNow,
                    TongTien = model.TongTien,
                    VAT = model.TamTinh * 0.08m
                },

                // 5.4. ⭐ THÊM MỚI: Tạo Vận Chuyển (Bảng VanChuyen)
                VanChuyen = new VanChuyen
                {
                    // (EF Core sẽ tự gán MaDonHang)
                    DonViVanChuyen = "ViettelPost", // (Hoặc tên dịch vụ bạn dùng)
                    MaVanDon = "VT001",
                    PhiVanChuyen = model.PhiVanChuyen,
                    TrangThaiGiaoHang = TrangThaiGiaoHang.ChoXuLy,
                    NgayDuKienGiao = DateTime.UtcNow.AddDays(3)
                }
            };

            // 5.5. Thêm Chi Tiết Đơn Hàng VÀ Trừ Tồn Kho
            foreach (var item in gioHangThanhToan)
            {
                // Thêm Chi Tiết
                donHang.ChiTietDonHangs.Add(new ChiTietDonHang
                {
                    MaSach = item.MaSach,
                    SoLuong = item.SoLuong,
                    DonGia = item.GiaSauGiam,
                    ThanhTien = item.ThanhTien
                });

                // Trừ Tồn Kho
                var sanPhamTrongKho = _unit.Saches.GetById(item.MaSach);
                // ⭐ SỬA LỖI LOGIC: Giả sử thuộc tính tồn kho là 'SoLuongTon'
                sanPhamTrongKho.SoLuong -= item.SoLuong;
                _unit.Saches.Update(sanPhamTrongKho);
            }

            // === BƯỚC 4: LƯU VÀ DỌN DẸP ===
            try
            {
                // 4.1. LƯU TẤT CẢ VÀO DATABASE
                // (UnitOfWork sẽ đảm bảo tất cả (DonHang, ChiTiet, HoaDon)
                // được lưu trong cùng 1 transaction)
                _unit.DonHangs.Add(donHang);
                await _unit.SaveAsync();

                // 4.2. DỌN DẸP GIỎ HÀNG (Yêu cầu của bạn)
                // Xóa giỏ hàng chờ thanh toán
                HttpContext.Session.Remove("GioHangThanhToan");

                // Lấy giỏ hàng chính (để cập nhật header)
                var gioHangChinh = HttpContext.Session.GetObjectFromJson<List<GioHangVM>>("GioHang");
                bool daXoaSanPham = false;
                if (gioHangChinh != null && gioHangChinh.Any())
                {
                    // Lấy danh sách MaSach đã mua
                    var maSachDaMua = new HashSet<int>(gioHangThanhToan.Select(i => i.MaSach));
                    int soLuongBanDau = gioHangChinh.Count();

                    // Xóa các sản phẩm đã mua khỏi giỏ hàng chính
                    gioHangChinh.RemoveAll(item => maSachDaMua.Contains(item.MaSach));
                    if (soLuongBanDau > gioHangChinh.Count()) daXoaSanPham = true;

                    // Lưu lại giỏ hàng chính (đã được cập nhật) vào Session
                    HttpContext.Session.SetObjectAsJson("GioHang", gioHangChinh);
                }
                if (daXoaSanPham) _gioHangService.LuuGioHangVaoDb(userId, gioHangChinh);

                // 4.3. CHUYỂN HƯỚNG
                TempData["MaDonHang"] = donHang.MaDonHang; // Gửi mã đơn hàng sang trang success
                return RedirectToAction("TrangChu", "Home", new { area = "Customer" });
            }
            catch (Exception ex)
            {
                // Xử lý lỗi nếu lưu DB thất bại
                TempData["ErrorMessage"] = "Đã xảy ra lỗi nghiêm trọng khi tạo đơn hàng. Vui lòng thử lại.";
                // Tải lại dữ liệu cho View
                model.DanhSachTinhThanh = await _locationService.GetProvincesAsync();
                var diaChiList = _unit.DiaChiNhanHangs.GetRange(dc => dc.MaKhachHang == khachHang.MaKhachHang).ToList();
                model.DanhSachDiaChi = diaChiList;
                model.DiaChiMacDinh = diaChiList.FirstOrDefault(d => d.LaMacDinh) ?? diaChiList.FirstOrDefault();
                return View(model);
            }
        }

        [HttpGet]
        public IActionResult ReLoadDiaChiNhanHangThanhToan()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var khachHang = _unit.KhachHangs.Get(kh => kh.MaTaiKhoan == userId);

            // Tải địa chỉ mặc định MỚI NHẤT
            var diaChiMacDinh = _unit.DiaChiNhanHangs
                .Get(dc => dc.MaKhachHang == khachHang.MaKhachHang && dc.LaMacDinh);

            // Nếu không có cái nào mặc định, lấy cái đầu tiên
            if (diaChiMacDinh == null)
            {
                diaChiMacDinh = _unit.DiaChiNhanHangs.Get(dc => dc.MaKhachHang == khachHang.MaKhachHang);
            }

            // Trả về Partial View với model là địa chỉ vừa tìm được
            return PartialView("_DiaChiNhanHangThanhToanPartial", diaChiMacDinh);
        }
    }
}