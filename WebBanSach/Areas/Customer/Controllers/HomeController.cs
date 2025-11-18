using Media.DataAccess.Repository;
using Media.DataAccess.Repository.IRepository;
using Media.Models;
using Media.Models.ViewModels;
using Media.Service;
using Media.Service.IServices;
using Media.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using NuGet.Protocol;
using System.Collections.Generic;
using System.Diagnostics;
using System.Security.Claims;
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
        private ISlugService _slugService;
        private readonly IViewRenderService _viewRenderService;
        private readonly IUnitOfWork _unit;
        private readonly IGioHangService _gioHangService;
        private readonly SignInManager<TaiKhoan> _signInManager;
        public HomeController(IUnitOfWork unit, 
                              LocationService locationService, 
                              IViewRenderService viewRenderService, 
                              ISlugService slugService,
                              IGioHangService gioHangService,
                              SignInManager<TaiKhoan> signInManager)
        private readonly IGioHangService _gioHangService;
        private readonly SignInManager<TaiKhoan> _signInManager;
        public HomeController(UserManager<TaiKhoan> taiKhoan, IUnitOfWork unit, LocationService locationService, IGioHangService gioHangService, SignInManager<TaiKhoan> signInManager)
        {
            _unit = unit;
            _slugService = slugService;
            _locationService = locationService;
            _viewRenderService = viewRenderService;
            _gioHangService = gioHangService;
            _signInManager = signInManager;
            _gioHangService = gioHangService;
            _signInManager = signInManager;
        }

        #region Trang chủ
        [HttpGet("")]
        public async Task<IActionResult> TrangChu()
        public IActionResult TrangChu()
        {
            var gioHang = HttpContext.Session.GetObjectFromJson<List<GioHangVM>>("GioHang") ?? new List<GioHangVM>();
            if (User.Identity.IsAuthenticated)
            var gioHang = HttpContext.Session.GetObjectFromJson<List<GioHangVM>>("GioHang") ?? new List<GioHangVM>();
            if (User.Identity.IsAuthenticated)
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                gioHang = _gioHangService.TaiGioHangTuDb(userId);
                HttpContext.Session.SetObjectAsJson("GioHang", gioHang);
            }

            TrangChuVM viewModel = new TrangChuVM()
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                gioHang = _gioHangService.TaiGioHangTuDb(userId);
                HttpContext.Session.SetObjectAsJson("GioHang", gioHang);
            }

            TrangChuVM viewModel = await GetIndexVM();
            return View(viewModel);
        }

        private async Task<TrangChuVM> GetIndexVM()
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
            TrangChuVM viewModel = new TrangChuVM();

            viewModel.SachBanChay = await _unit.Saches.LaySachBanChay(days: 7, sachCount: 6);

            viewModel.TacGiaNoiTieng = (viewModel.SachBanChay).Select(s => s.TacGia)
                    .GroupBy(tg => tg.MaTacGia)
                    .Select(g => g.First())
                    .ToList();

            viewModel.TuSachMienPhi = _unit.Saches.GetRange(s => s.GiaBan == 0).Take(12);

            List<ChuDe> listChuDe = _unit.ChuDes.GetRange(cd => cd.ParentId == 1).Take(4).ToList();
            listChuDe.AddRange(_unit.ChuDes.GetRange(cd => cd.ParentId == 2).Take(5));

            foreach (ChuDe cd in listChuDe)
            {
                string duongDanHinhAnh;

                switch (cd.TenChuDe)
                {
                    case "Văn Học":
                        {
                            duongDanHinhAnh = "/img/product/van_hoc.jpg";
                            break;
                        }
                    case "Kinh Tế":
                        {
                            duongDanHinhAnh = "/img/product/kinh_te.jpg";
                            break;
                        }
                    case "Tâm lý - Kĩ Năng Sống":
                        {
                            duongDanHinhAnh = "/img/product/tam_ly.jpg";
                            break;
                        }
                    case "Sách Thiếu Nhi":
                        {
                            duongDanHinhAnh = "/img/product/doraemon-1.jpg";
                            break;
                        }
                    case "Fiction":
                        {
                            duongDanHinhAnh = "/img/product/fiction.jpg";
                            break;
                        }
                    case "Non-Fiction":
                        {
                            duongDanHinhAnh = "/img/product/non_fiction.jpg";
                            break;
                        }
                    case "English Learning":
                        {
                            duongDanHinhAnh = "/img/product/english.jpg";
                            break;
                        }
                    case "Children Books":
                        {
                            duongDanHinhAnh = "/img/product/children_books.jpg";
                            break;
                        }
                    case "Comics & Graphic Novels":
                    default:
                        {
                            duongDanHinhAnh = "/img/product/batman-killing-joke.jpg";
                            break;
                        }
                }
            var sach = _unit.Saches.Get(s => s.MaSach == maSach, includeProperties: "ChuDe,NhaXuatBan,TacGia");
            if (sach == null)
                return Json(false);

            bool isAvailable = soLuongMuonTang <= sach.SoLuong;
            return Json(new { success = isAvailable });
        }

                viewModel.DanhSachChuDe.Add(new DanhSachChuDeTrangIndex
                {
                    ChuDeFullPath = cd.FullPath,
                    TenChuDe = cd.TenChuDe,
                    DuongDanHinhAnh = duongDanHinhAnh
                });
            }

            return viewModel;
        }
        #endregion

        private async Task<IActionResult> RenderSachListView(SachTheoChuDeVM viewModel, bool includeChudeTuongUng = false)
        {
            bool isAjax = Request.Headers["X-Requested-With"] == "XMLHttpRequest";
            if (isAjax)
            {
                var response = new Dictionary<string, object>
                {
                    ["success"] = true,
                    ["sachList"] = await _viewRenderService.RenderToStringAsync("_SachListPartial", viewModel),
                    ["toolbar"] = await _viewRenderService.RenderToStringAsync("_ToolbarPartial", viewModel),
                    ["pagination"] = await _viewRenderService.RenderToStringAsync("_PaginationPartial", viewModel.DanhSachSach),
                    ["totalSaches"] = viewModel.DanhSachSach.TotalItemCount,
                    ["currentPage"] = viewModel.DanhSachSach.PageNumber,
                    ["totalPages"] = viewModel.DanhSachSach.PageCount
                };

                if (viewModel.DanhSachTacGia != null)
                {
                    response["tacGiaList"] = await _viewRenderService.RenderToStringAsync(
                        "_TacGiaFilterPartial",
                        viewModel.DanhSachTacGia
                    );
                }

                if (viewModel.DanhSachNhaXuatBan != null)
                {
                    response["nhaXuatBanList"] = await _viewRenderService.RenderToStringAsync(
                        "_NhaXuatBanFilterPartial",
                        viewModel.DanhSachNhaXuatBan
                    );
                }

                if (includeChudeTuongUng && viewModel.ChuDeCha != null)
                {
                    response["chuDeTuongUng"] = await _viewRenderService.RenderToStringAsync(
                        "_ChuDeTuongUngPartial",
                        viewModel.ChuDeCha
                    );
                }

                return Json(response);
            }

            return View("SachTheoChuDe", viewModel);
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

        public async Task<IActionResult> TimKiemAsync(string? keyword)
        {
            SachTheoChuDeVM viewModel = await LoadSachTheoChuDeVMAsync(0, 1, 12, null, null, null, null, keyword);

            viewModel.DanhSachTacGia = viewModel.DanhSachSach.Select(s => s.TacGia)
                    .GroupBy(tg => tg.MaTacGia)
                    .Select(g => g.First())
                    .ToList(); ;
            viewModel.DanhSachNhaXuatBan = viewModel.DanhSachSach.Select(s => s.NhaXuatBan)
                                    .GroupBy(nxb => nxb.MaNhaXuatBan)
                                    .Select(g => g.First())
                                    .ToList();
            viewModel.Breadcrumbs = new List<BreadcrumbItem>(){
                new BreadcrumbItem { Text = "Trang chủ", Url = "/" },
                new BreadcrumbItem { Text = "Tìm kiếm bằng từ khóa", Url = "/", IsActive = true}
            };

            return await RenderSachListView(viewModel);
        }

        public async Task<IActionResult> LayTatCaSach(int? page, int? pageSize)
        {
            SachTheoChuDeVM viewModel = await LoadSachTheoChuDeVMAsync(0, page ?? 1, pageSize ?? 12, null, null, null, null, null);

            viewModel.DanhSachTacGia = await _unit.TacGias.GetAllReadOnlyAsync();
            viewModel.DanhSachNhaXuatBan = await _unit.NhaXuatBans.GetAllReadOnlyAsync();
            viewModel.Breadcrumbs = new List<BreadcrumbItem>(){
                new BreadcrumbItem { Text = "Trang chủ", Url = "/" },
                new BreadcrumbItem { Text = "Tất cả chủ đề", Url = "/", IsActive = true}
            };

            return await RenderSachListView(viewModel);
        }

        public async Task<IActionResult> LaySachTheoTacGia(int id)
        {
            SachTheoChuDeVM viewModel = await LoadSachTheoChuDeVMAsync(0, 1, 12, null, new List<int> { id }, null, null, null);

            TacGia selectedTacGia = _unit.TacGias.Get(tg => tg.MaTacGia == id);

            viewModel.DanhSachTacGia = new List<TacGia> { selectedTacGia };

            viewModel.DanhSachNhaXuatBan = viewModel.DanhSachSach.Select(s => s.NhaXuatBan)
                                    .GroupBy(nxb => nxb.MaNhaXuatBan)
                                    .Select(g => g.First())
                                    .ToList();

            viewModel.Breadcrumbs = new List<BreadcrumbItem>(){
                new BreadcrumbItem { Text = "Trang chủ", Url = "/" },
                new BreadcrumbItem { Text = $"{viewModel.DanhSachTacGia.First().TenTG}", Url = "/", IsActive = true}
            };

            return await RenderSachListView(viewModel);
        }

        #region Sách Theo Chủ Đề

        [Route("chude/{*path}")]
        public async Task<IActionResult> SachTheoChuDe(
            string path,
            int? page,
            int? pageSize,
            List<string> priceRanges, 
            List<int> tacGiaIds,
            List<int> nhaXuatBanIds,
            string? sortBy,
            string? keyword)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                return RedirectToAction("LayTatCaSach");
            }

            path = Uri.UnescapeDataString(path);

            ChuDe? selectedChuDe = _slugService.GetChuDeByPath(path);
            
            if (selectedChuDe is null)
            {
                selectedChuDe = new ChuDe()
                {
                    MaChuDe = 0
                };
            }

            SachTheoChuDeVM viewModel = await LoadSachTheoChuDeVMAsync(
                selectedChuDe.MaChuDe, page ?? 1, pageSize ?? 12, priceRanges, tacGiaIds, nhaXuatBanIds, sortBy, keyword);

            viewModel.Breadcrumbs = BuildBreadcrumbs(selectedChuDe);
            viewModel.ChuDeSelected = selectedChuDe;
            ViewBag.CurrentPath = path;
            ViewBag.SearchKeyword = keyword;

            return await RenderSachListView(viewModel, includeChudeTuongUng: true);
        }

        private async Task<SachTheoChuDeVM> LoadSachTheoChuDeVMAsync(
            int chuDeId, 
            int page,
            int pageSize,
            List<string>? priceRanges,
            List<int>? tacGiaIds,
            List<int>? nhaXuatBanIds,
            string? sortBy,
            string? keyword)
        {
            IQueryable<Sach> query;

            SachTheoChuDeVM viewModel = new SachTheoChuDeVM();

            // ===== LỌC THEO CHỦ ĐỀ =====
            if (chuDeId == 0) // Chủ đề là 0 thì tức là lấy tất cả chủ đề
            {
                ChuDe allParentChuDe = new ChuDe();
                allParentChuDe.Children = await _unit.ChuDes.GetRangeReadOnlyAsync(cd => cd.ParentId == null);
                viewModel.ChuDeCha = allParentChuDe;
                viewModel.ChuDeSelected = new ChuDe() { MaChuDe = 0};
                query = _unit.Saches.GetAll().AsQueryable()
                    .Include(s => s.TacGia)
                    .Include(s => s.NhaXuatBan)
                    .Include(s => s.ChuDe);
            }
            else
            {
                ChuDe? selectedChuDe = _unit.ChuDes.Get(cd => cd.MaChuDe == chuDeId);
                viewModel.ChuDeCha = await LayChuDeCha(selectedChuDe);
                viewModel.ChuDeSelected = selectedChuDe;
                query = _unit.Saches.LaySachTheoChuDe(chuDeId);
            }

            // ===== LỌC THEO TỪ KHÓA =====
            if (!string.IsNullOrWhiteSpace(keyword))
            {
                string searchTerm = keyword.ToLower().Trim();
                query = query.Where(s =>
                    s.TenSach.ToLower().Contains(searchTerm) ||
                    s.TacGia.TenTG.ToLower().Contains(searchTerm));

                viewModel.SearchKeyword = keyword;
            }

            // ===== LỌC THEO GIÁ =====
            viewModel.PriceFilter = new PriceFilter()
            {
                PriceRanges = LoadPriceRanges(),
                SelectedRanges = priceRanges ?? new List<string>()
            };

            if (!priceRanges.IsNullOrEmpty() && priceRanges.Any())
            {
                query = query.Where(s =>
                    (priceRanges.Contains("range1") && s.GiaBan >= 0 && s.GiaBan < 150000) ||
                    (priceRanges.Contains("range2") && s.GiaBan >= 150000 && s.GiaBan < 300000) ||
                    (priceRanges.Contains("range3") && s.GiaBan >= 300000 && s.GiaBan < 500000) ||
                    (priceRanges.Contains("range4") && s.GiaBan >= 500000 && s.GiaBan < 700000) ||
                    (priceRanges.Contains("range5") && s.GiaBan >= 700000)
                );
            }

            // ===== LỌC THEO TÁC GIẢ =====
            if(tacGiaIds is not null && tacGiaIds.Any())
            {
                query = query.Where(s => tacGiaIds.Contains(s.MaTacGia));
            }


            // ===== LỌC THEO NHÀ XUẤT BẢN =====
            if (nhaXuatBanIds is not null && nhaXuatBanIds.Any())
            {
                query = query.Where(s => nhaXuatBanIds.Contains(s.MaNhaXuatBan));
            }

            // ===== LỌC THEO SẮP XẾP =====
            query = sortBy switch
            {
                "price-asc" => query.OrderBy(s => s.GiaBan),
                "price-desc" => query.OrderByDescending(s => s.GiaBan),
                "name-asc" => query.OrderBy(s => s.TenSach),
                "name-desc" => query.OrderByDescending(s => s.TenSach),
                "newest" => query.OrderByDescending(s => s.NgayCapNhat),
                "oldest" => query.OrderBy(s => s.NgayCapNhat),
                _ => query.OrderBy(s => s.TenSach)
            };


            List<Sach> sachList = await query.ToListAsync();

            if(chuDeId != 0)
            {
                viewModel.DanhSachTacGia = sachList.Select(s => s.TacGia)
                    .GroupBy(tg => tg.MaTacGia)
                    .Select(g => g.First())
                    .ToList();

                viewModel.DanhSachNhaXuatBan = sachList.Select(s => s.NhaXuatBan)
                                        .GroupBy(nxb => nxb.MaNhaXuatBan)
                                        .Select(g => g.First())
                                        .ToList();
            }

            viewModel.DanhSachSach = sachList.ToPagedList(page, pageSize);
            viewModel.SortBy = sortBy;
            viewModel.PageSize = pageSize;
            return viewModel;
        }

        private List<PriceRange> LoadPriceRanges()
        {
            return new List<PriceRange>()
            {
                new PriceRange { Id = "range1", Label = "0đ - 150.000đ", MinPrice = 0, MaxPrice = 150000},
                new PriceRange { Id = "range2", Label = "150,000đ - 300,000đ", MinPrice = 150000, MaxPrice = 300000 },
                new PriceRange { Id = "range3", Label = "300,000đ - 500,000đ", MinPrice = 300000, MaxPrice = 500000 },
                new PriceRange { Id = "range4", Label = "500,000đ - 700,000đ", MinPrice = 500000, MaxPrice = 700000 },
                new PriceRange { Id = "range5", Label = "700,000đ - Trở Lên", MinPrice = 700000, MaxPrice = null }
            };
        }
                TenSanPham = "Bộ Manga - Summer Ghost - Bóng Ma Mùa Hạ - Tập 1 + Tập 2",
                HinhAnhSanPham = "/images/product.jpg",
                SoLuong = 1,
                TamTinh = 76000,
                PhiVanChuyen = 20000,
                TongTien = 96000,

        private async Task<ChuDe?> LayChuDeCha(ChuDe selectedChuDe)
        {
            ChuDe? parentChuDe = _unit.ChuDes.Get(cd => cd.MaChuDe == selectedChuDe.ParentId);

            selectedChuDe.Children = await _unit.ChuDes.GetRangeReadOnlyAsync(cd => cd.ParentId == selectedChuDe.MaChuDe);
            if(parentChuDe is null)
            {
                return selectedChuDe;
            }

            parentChuDe.Children = new List<ChuDe>() { selectedChuDe };

            return await LayChuDeCha(parentChuDe);
        }

        private List<BreadcrumbItem> BuildBreadcrumbs(ChuDe chuDe)
        {
            var breadcrumbs = new List<BreadcrumbItem>
            {
                new BreadcrumbItem { Text = "Trang chủ", Url = "/" }
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

            if(chuDe.MaChuDe == 0)
            {
                return breadcrumbs;
            }


        #region Địa chỉ nhận hàng
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
        #endregion


        public IActionResult OrderSuccess()
        {
            return View();
        }
    }
}

