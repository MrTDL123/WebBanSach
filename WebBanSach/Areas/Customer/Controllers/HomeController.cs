using Media.DataAccess.Repository;
using Media.DataAccess.Repository.IRepository;
using Media.Models;
using Media.Models.ViewModels;
using Media.Service;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text.Json;
using System.Threading.Tasks;
using X.PagedList;
using X.PagedList.Extensions;
using Media.Utility;

namespace ProjectCuoiKi.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class HomeController : Controller
    {
        private readonly LocationService _locationService;
        private readonly IUnitOfWork _unit;
        private readonly IGioHangService _gioHangService;
        private readonly SignInManager<TaiKhoan> _signInManager;
        public HomeController(UserManager<TaiKhoan> taiKhoan, IUnitOfWork unit, LocationService locationService, IGioHangService gioHangService, SignInManager<TaiKhoan> signInManager)
        {
            _unit = unit;
            _locationService = locationService;
            _gioHangService = gioHangService;
            _signInManager = signInManager;
        }

        public IActionResult TrangChu()
        {
            var gioHang = HttpContext.Session.GetObjectFromJson<List<GioHangVM>>("GioHang") ?? new List<GioHangVM>();
            if (User.Identity.IsAuthenticated)
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                gioHang = _gioHangService.TaiGioHangTuDb(userId);
                HttpContext.Session.SetObjectAsJson("GioHang", gioHang);
            }

            TrangChuVM viewModel = new TrangChuVM()
            {
                DanhSachSanPham = _unit.Saches
                              .GetAll(includeProperties: "ChuDe,NhaXuatBan,TacGia")
                              .OrderByDescending(s => s.GiaBan)
                              .Take(5)
                              .ToList(),
                DanhSachChuDe = _unit.ChuDes.GetAll()
            };
            
            return View(viewModel);
        }

        // (Nhớ inject: IUnitOfWork _unit, SignInManager<TaiKhoan> _signInManager, IEmailSender _emailSender...)

        // 1. [HttpGet] Tải Trang Chi Tiết (ĐÃ SỬA LỖI - THÊM INCLUDE)
        [HttpGet]
        public async Task<IActionResult> ChiTietSanPham(int maSach)
        {
            if (!_signInManager.IsSignedIn(User))
            {
                TempData["ReturnUrl"] = Url.Action("ChiTietSanPham", "Home", new { maSach = maSach });
            }
            // ⭐ SỬA LỖI TẠI ĐÂY:
            // Dùng GetAsync và thêm "includeProperties" để tải các bảng liên quan
            var sach = await _unit.Saches.GetAsync(
                s => s.MaSach == maSach,
                includeProperties: "ChuDe,NhaXuatBan,TacGia"
            );

            if (sach == null) { return NotFound(); }

            // (Code tải đánh giá để tính toán % sao)
            var danhSachDanhGia = (await _unit.DanhGiaSanPhams.GetRangeAsync(
                            d => d.MaSach == maSach
                         )).ToList();

            var vm = new ChiTietSanPhamVM
            {
                Sach = sach, // (Bây giờ đã chứa Tác Giả, NXB, Chủ Đề)
                TongSoDanhGiaSanPham = danhSachDanhGia.Count
            };

            // Tính toán % sao và điểm trung bình
            vm.PhanTramTheoSoSao = new Dictionary<int, int> { { 5, 0 }, { 4, 0 }, { 3, 0 }, { 2, 0 }, { 1, 0 } };

            if (vm.TongSoDanhGiaSanPham > 0)
            {
                vm.DiemDanhGiaSanPhamTrungBinh = danhSachDanhGia.Average(d => d.SoSao);
                for (int i = 5; i >= 1; i--)
                {
                    int count = danhSachDanhGia.Count(d => d.SoSao == i);
                    vm.PhanTramTheoSoSao[i] = (int)Math.Round((double)count * 100 / vm.TongSoDanhGiaSanPham);
                }
            }
            else
            {
                vm.DiemDanhGiaSanPhamTrungBinh = 5; // Mặc định
            }

            return View(vm);
        }

        // 2. [HttpGet] Tải Danh Sách Đánh Giá (Cho Tab)
        // (Action này KHÔNG cần 'includeProperties' cho Sách)
        [HttpGet]
        public async Task<IActionResult> TaiDanhGiaPartial(int maSach, string kieuSapXep = "MoiNhat", int trang = 1)
        {
            const int KICH_THUOC_TRANG = 5;
            int? maKhachHangHienTai = null;
            if (_signInManager.IsSignedIn(User))
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var khachHang = _unit.KhachHangs.Get(kh => kh.MaTaiKhoan == userId);
                if (khachHang != null) maKhachHangHienTai = khachHang.MaKhachHang;
            }

            var danhSachDaThich = new HashSet<int>();
            if (maKhachHangHienTai.HasValue)
            {
                danhSachDaThich = (await _unit.LuotThichDanhGiaSanPhams.GetRangeAsync(
                    lt => lt.MaKhachHang == maKhachHangHienTai.Value
                )).Select(lt => lt.MaDanhGia).ToHashSet();
            }

            Func<IQueryable<DanhGiaSanPham>, IOrderedQueryable<DanhGiaSanPham>> orderBy;
            if (kieuSapXep == "YeuThichNhat")
            {
                orderBy = q => q.OrderByDescending(d => d.LuotThich).ThenByDescending(d => d.NgayDang);
            }
            else
            {
                orderBy = q => q.OrderByDescending(d => d.NgayDang);
            }

            var danhSachDanhGia = await _unit.DanhGiaSanPhams.GetRangeAsync(
                d => d.MaSach == maSach,
                orderBy: orderBy,
                includeProperties: "KhachHang" // Chỉ cần include KhachHang để lấy tên
            );

            var danhGiaTheoTrang = danhSachDanhGia
                .Skip((trang - 1) * KICH_THUOC_TRANG)
                .Take(KICH_THUOC_TRANG)
                .ToList();

            ViewBag.DanhSachDaThich = danhSachDaThich;

            return PartialView("_DanhSachDanhGiaPartial", danhGiaTheoTrang);
        }

        // 3. [HttpPost] Xử lý Thích/Bỏ Thích
        // (Action này KHÔNG cần 'includeProperties' cho Sách)
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ThichDanhGia(int maDanhGia)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var khachHang = _unit.KhachHangs.Get(kh => kh.MaTaiKhoan == userId);
            if (khachHang == null) { return Unauthorized(); }

            var maKhachHangHienTai = khachHang.MaKhachHang;

            var danhGia = await _unit.DanhGiaSanPhams.GetByIdAsync(maDanhGia);
            if (danhGia == null) { return NotFound(); }

            var luotThichDaTonTai = await _unit.LuotThichDanhGiaSanPhams.GetAsync(
                lt => lt.MaKhachHang == maKhachHangHienTai && lt.MaDanhGia == maDanhGia
            );

            bool daThich;
            if (luotThichDaTonTai != null)
            {
                _unit.LuotThichDanhGiaSanPhams.Remove(luotThichDaTonTai);
                danhGia.LuotThich = Math.Max(0, danhGia.LuotThich - 1);
                daThich = false;
            }
            else
            {
                _unit.LuotThichDanhGiaSanPhams.Add(new LuotThichDanhGiaSanPham
                {
                    MaKhachHang = maKhachHangHienTai,
                    MaDanhGia = maDanhGia
                });
                danhGia.LuotThich = danhGia.LuotThich + 1;
                daThich = true;
            }

            _unit.DanhGiaSanPhams.Update(danhGia);
            await _unit.SaveAsync();

            return Json(new
            {
                success = true,
                soLuotThichMoi = danhGia.LuotThich,
                daThich = daThich
            });
        }

        // 4. [HttpGet] Tải Popup Viết Đánh Giá
        // (Action này KHÔNG cần 'includeProperties' cho Sách,
        //  vì popup chỉ cần MaSach để submit)
        [HttpGet]
        public IActionResult VietDanhGiaPartial(int maSach)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var khachHang = _unit.KhachHangs.Get(kh => kh.MaTaiKhoan == userId);

            var model = new DanhGiaSanPham
            {
                MaSach = maSach,
                TenHienThi = khachHang?.HoTen ?? User.Identity.Name
            };

            return PartialView("_PopupVietDanhGiaSanPham", model);
        }

        // 5. [HttpPost] Gửi Đánh Giá Mới
        // (Action này KHÔNG cần 'includeProperties' cho Sách)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> GuiDanhGia(DanhGiaSanPham model)
        {
            if (!_signInManager.IsSignedIn(User)) { return Unauthorized(); }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var khachHang = _unit.KhachHangs.Get(kh => kh.MaTaiKhoan == userId);

            if (!ModelState.IsValid)
            {
                return PartialView("_PopupVietDanhGiaSanPham", model);
            }

            model.MaKhachHang = khachHang.MaKhachHang;
            model.NgayDang = DateTime.UtcNow;

            _unit.DanhGiaSanPhams.Add(model);
            await _unit.SaveAsync();

            return Json(new { success = true, message = "Gửi đánh giá thành công!" });
        }

        [HttpGet]
        public IActionResult KiemTraTonKho(int maSach, int soLuongMuonTang)
        {
            var sach = _unit.Saches.Get(s => s.MaSach == maSach, includeProperties: "ChuDe,NhaXuatBan,TacGia");
            if (sach == null)
                return Json(false);

            bool isAvailable = soLuongMuonTang <= sach.SoLuong;
            return Json(new { success = isAvailable });
        }

        #region Sách Theo Chủ Đề
        public async Task<IActionResult> LayTatCaSach(int? page)
        {
            int pageNumber = page ?? 1;
            ViewBag.Url = "Trang chủ > Tất cả chủ đề";


            SachTheoChuDeVM viewModel = new SachTheoChuDeVM();

            ChuDe allParentChuDe = new ChuDe();
            allParentChuDe.Children = await _unit.ChuDes.GetRangeReadOnly(cd => cd.ParentId == null);
            viewModel.ChuDeHienTai = allParentChuDe;
            List<Sach> danhSachSach = _unit.Saches.GetAll().ToList();
            viewModel.DanhSachSach = danhSachSach.ToPagedList(pageNumber, 20);
            viewModel.DanhSachTenNhaXuatBan = danhSachSach.Select(s => s.NhaXuatBan.TenNXB).Distinct();
            viewModel.DanhSachTenTacGia = danhSachSach.Select(s => s.TacGia.TenTG).Distinct();

            return View("SachTheoChuDe", viewModel);
        }

        public async Task<IActionResult> SachTheoChuDe(int? id, int? page)
        {                
            SachTheoChuDeVM viewModel = new SachTheoChuDeVM();
            int pageNumber = page ?? 1;
            ViewBag.Url = LayURL(id, new List<string>());

            
            ChuDe selectedChuDe = _unit.ChuDes.Get(cd => cd.MaChuDe == id);
            if (selectedChuDe.ParentId == null)
            {
                ChuDe topLevelChuDe = new ChuDe();
                topLevelChuDe.Children = await _unit.ChuDes.GetRangeReadOnly(cd => cd.ParentId == null);
                viewModel.ChuDeHienTai = topLevelChuDe;
            }
            else
            {
                viewModel.ChuDeHienTai = await LayChuDeLevel1(selectedChuDe);
            }
            List<Sach>? danhSachSach = await _unit.Saches.LaySachTheoChuDe(id);
            viewModel.DanhSachSach = danhSachSach.ToPagedList(pageNumber, 20);
            viewModel.DanhSachTenNhaXuatBan = danhSachSach.Select(s => s.NhaXuatBan.TenNXB).Distinct();
            viewModel.DanhSachTenTacGia = danhSachSach.Select(s => s.TacGia.TenTG).Distinct();

            return View(viewModel);
        }

        private async Task<ChuDe?> LayChuDeLevel1(ChuDe selectedChuDe)
        {

            ChuDe? parentChuDe = _unit.ChuDes.Get(cd => cd.MaChuDe == selectedChuDe.ParentId);
            if(parentChuDe is null)
            {
                return selectedChuDe;
            }

            selectedChuDe.Children = await _unit.ChuDes.GetRangeReadOnly(cd => cd.ParentId == selectedChuDe.MaChuDe);
            parentChuDe.Children = new List<ChuDe>() { selectedChuDe };

            return await LayChuDeLevel1(parentChuDe);
        }


        private string LayURL(int? maCD, List<string> url)
        {
            ChuDe? selectedChuDe = _unit.ChuDes.Get(cd => cd.MaChuDe == maCD);

            if(selectedChuDe.ParentId is not null)
            {
                LayURL(selectedChuDe.ParentId, url);
            }
            else
            {
                url.Add("Trang chủ");
            }

            url.Add(" > "  + selectedChuDe.TenChuDe);

            return String.Join("", url);
        }

        public async Task<IActionResult> ChangeAddressPartial()
        {
            var provinces = await _locationService.GetProvincesAsync();

            var model = new ThanhToan
            {
                TenSanPham = "Bộ Manga - Summer Ghost - Bóng Ma Mùa Hạ - Tập 1 + Tập 2",
                HinhAnhSanPham = "/images/product.jpg",
                SoLuong = 1,
                TamTinh = 76000,
                PhiVanChuyen = 20000,
                TongTien = 96000,

                DanhSachTinhThanh = provinces, // Lấy từ API thật
                DanhSachQuanHuyen = new List<SelectListItem>
                {
                    new SelectListItem { Value = "", Text = "Chọn quận/huyện", Disabled = true, Selected = true }
                },
                DanhSachPhuongXa = new List<SelectListItem>
                {
                    new SelectListItem { Value = "", Text = "Chọn phường/xã", Disabled = true, Selected = true }
                }
            };
            return PartialView("_PopupThayDoiDiaChiGiaoHang", model);
        }

        [HttpGet]
        public async Task<JsonResult> GetProvinces()
        {
            var provinces = await _locationService.GetProvincesAsync();
            return Json(provinces);
        }


        [HttpGet]
        public async Task<JsonResult> GetDistricts(int provinceCode)
        {
            var districts = await _locationService.GetDistrictsAsync(provinceCode);
            return Json(districts);
        }

        [HttpGet]
        public async Task<JsonResult> GetWards(int districtCode)
        {
            var wards = await _locationService.GetWardsAsync(districtCode);
            return Json(wards);
        }

        public IActionResult OrderSuccess()
        {
            return View();
        }
    }
}

#endregion