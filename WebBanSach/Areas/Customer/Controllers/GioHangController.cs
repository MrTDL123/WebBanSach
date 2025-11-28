using Media.DataAccess.Repository.IRepository;
using Media.Models.ViewModels;
using Media.Service;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Media.Utility;
using Media.Models;
using System.Threading.Tasks;

namespace WebBanSach.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class GioHangController : Controller
    {
        private readonly IUnitOfWork _unit;
        private readonly IGioHangService _gioHangService;
        public GioHangController(IUnitOfWork unit, IGioHangService gioHangService)
        {
            _unit = unit;
            _gioHangService = gioHangService;
        }

        [HttpGet]
        public IActionResult GioHang()
        {
            var gioHang = new List<GioHangVM>();

            if (User.Identity.IsAuthenticated)
            {
                // 1. Nếu đã đăng nhập, LUÔN LẤY TỪ DB ĐỂ ĐỒNG BỘ
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                gioHang = _gioHangService.TaiGioHangTuDb(userId);

                // 2. Tải xong thì cập nhật lại Session (làm mới cache)
                HttpContext.Session.SetObjectAsJson("GioHang", gioHang);
            }
            else
            {
                // 3. Nếu là Guest, chỉ lấy từ Session
                gioHang = HttpContext.Session.GetObjectFromJson<List<GioHangVM>>("GioHang") ?? new List<GioHangVM>();
            }

            return View(gioHang);
        }

        [HttpPost]
        public IActionResult ThemVaoGioHang(int maSach, int soLuong)
        {
            var sach = _unit.Saches.Get(s => s.MaSach == maSach);
            if (sach == null)
            {
                return NotFound();
            }

            // Lấy giỏ hàng hiện tại từ Session (nếu chưa có thì tạo mới)
            var gioHang = HttpContext.Session.GetObjectFromJson<List<GioHangVM>>("GioHang") ?? new List<GioHangVM>();

            // Kiểm tra sản phẩm đã có trong giỏ chưa
            var sanPham = gioHang.FirstOrDefault(x => x.MaSach == maSach);
            if (sanPham != null)
            {
                sanPham.SoLuong++;
            }
            else
            {
                gioHang.Add(new GioHangVM
                {
                    MaSach = sach.MaSach,
                    TenSach = sach.TenSach,
                    GiaBan = sach.GiaBan,
                    GiaSauGiam = sach.GiaSauGiam,
                    AnhBiaChinh = sach.AnhBiaChinh,
                    SoLuong = soLuong
                });
            }

            // Cập nhật lại session
            HttpContext.Session.SetObjectAsJson("GioHang", gioHang);

            // ✅ NẾU ĐÃ ĐĂNG NHẬP -> LƯU XUỐNG DB
            if (User.Identity.IsAuthenticated)
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                // Hàm này sẽ xóa giỏ hàng cũ của user và thêm lại giỏ hàng mới từ biến 'gioHang'
                _gioHangService.LuuGioHangVaoDb(userId, gioHang);
            }

            return Json(new { success = true, count = gioHang.Count() });
        }

        [HttpGet]
        public async Task<IActionResult> MuaLaiGioHang(int id)
        {
            DonHang donHangCu = _unit.DonHangs.Get(dh => dh.MaDonHang == id, includeProperties: "ChiTietDonHangs");
            if (donHangCu == null)
            {
                return NotFound();
            }

            var gioHang = HttpContext.Session.GetObjectFromJson<List<GioHangVM>>("GioHang") ?? new List<GioHangVM>();

            foreach (ChiTietDonHang chitietcu in donHangCu.ChiTietDonHangs)
            {
                var sachHienTai = _unit.Saches.Get(s => s.MaSach == chitietcu.MaSach);

                if(sachHienTai == null || sachHienTai.SoLuong == 0)
                {
                    continue;
                }

                var sachTrongGio = gioHang.FirstOrDefault(s => s.MaSach == sachHienTai.MaSach);

                if(sachTrongGio != null)
                {
                    sachTrongGio.SoLuong += chitietcu.SoLuong;
                }
                else
                {
                    gioHang.Add(new GioHangVM
                    {
                        MaSach = sachHienTai.MaSach,
                        TenSach = sachHienTai.TenSach,
                        GiaBan = sachHienTai.GiaBan,
                        GiaSauGiam = sachHienTai.GiaSauGiam,
                        AnhBiaChinh = sachHienTai.AnhBiaChinh,
                        SoLuong = chitietcu.SoLuong
                    });
                }
            }

            HttpContext.Session.SetObjectAsJson("GioHang", gioHang);

            // ✅ NẾU ĐÃ ĐĂNG NHẬP -> LƯU XUỐNG DB
            if (User.Identity.IsAuthenticated)
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                // Hàm này sẽ xóa giỏ hàng cũ của user và thêm lại giỏ hàng mới từ biến 'gioHang'
                _gioHangService.LuuGioHangVaoDb(userId, gioHang);
            }

            return RedirectToAction("GioHang");
        }

        [HttpPost]
        public IActionResult XoaKhoiGioHang(int maSach)
        {
            var gioHang = HttpContext.Session.GetObjectFromJson<List<GioHangVM>>("GioHang") ?? new List<GioHangVM>();

            var sanPham = gioHang.FirstOrDefault(c => c.MaSach == maSach);
            if (sanPham != null)
            {
                gioHang.Remove(sanPham);
                HttpContext.Session.SetObjectAsJson("GioHang", gioHang);
            }

            // 3. ✅ NẾU ĐÃ ĐĂNG NHẬP -> CẬP NHẬT LẠI DB
            // Hàm 'LuuGioHangVaoDb' sẽ xóa cũ và thêm mới, nên nó cũng xử lý
            // luôn trường hợp xóa
            if (User.Identity.IsAuthenticated)
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                _gioHangService.LuuGioHangVaoDb(userId, gioHang);
            }

            return Json(new
            {
                success = true,
                count = gioHang.Count()
            });
        }

        [HttpPost]
        public IActionResult CapNhatThanhTienTrongGioHang(int maSach, int soLuong)
        {
            var gioHang = HttpContext.Session.GetObjectFromJson<List<GioHangVM>>("GioHang") ?? new List<GioHangVM>();
            var sanPham = gioHang.FirstOrDefault(c => c.MaSach == maSach);

            if (sanPham != null)
            {
                sanPham.SoLuong = soLuong;
            }

            HttpContext.Session.SetObjectAsJson("GioHang", gioHang);

            // ✅ BỔ SUNG ĐỒNG BỘ DATABASE
            if (User.Identity.IsAuthenticated)
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                _gioHangService.LuuGioHangVaoDb(userId, gioHang);
            }

            return Json(new
            {
                success = true,
                thanhTien = sanPham.ThanhTien.ToString("N0") + " đ"
            });
        }
    }
}