using AspNetCoreGeneratedDocument;
using Media.DataAccess.Repository.IRepository;
using Media.Models;
using Media.Models.ViewModels;
using Media.Service;
using Media.Service.IServices;
using Media.Utility;
using Meida.DataAccess.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using NuGet.Protocol;
using System.Collections.Generic;
using System.Diagnostics;
using System.Security.Claims;
using System.Threading.Tasks;
using X.PagedList;
using X.PagedList.Extensions;

namespace ProjectCuoiKi.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly LocationService _locationService;
        private readonly ISlugService _slugService;
        private readonly ISachRepository _sachRepository;
        private readonly IViewRenderService _viewRenderService;
        private readonly IGioHangService _gioHangService;
        private readonly SignInManager<TaiKhoan> _signInManager;
        private readonly UserManager<TaiKhoan> _userManager;
        public HomeController(ApplicationDbContext context,
                              LocationService locationService, 
                              IViewRenderService viewRenderService, 
                              ISlugService slugService,
                              ISachRepository sachRepository,
                              IGioHangService gioHangService,
                              SignInManager<TaiKhoan> signInManager,
                              UserManager<TaiKhoan> userManager)
        {
            _context = context;
            _slugService = slugService;
            _sachRepository = sachRepository;
            _locationService = locationService;
            _viewRenderService = viewRenderService;
            _gioHangService = gioHangService;
            _signInManager = signInManager;
            _userManager = userManager;
        }

        #region Trang chủ
        [HttpGet("")]
        public async Task<IActionResult> TrangChu()
        {
            var gioHang = HttpContext.Session.GetObjectFromJson<List<GioHangVM>>("GioHang") ?? new List<GioHangVM>();
            if (User.Identity.IsAuthenticated)
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                gioHang = _gioHangService.TaiGioHangTuDb(userId);
                HttpContext.Session.SetObjectAsJson("GioHang", gioHang);
            }

            TrangChuVM viewModel = await GetIndexVM();
            return View(viewModel);
        }

        private async Task<TrangChuVM> GetIndexVM()
        {
            TrangChuVM viewModel = new TrangChuVM();
            viewModel.SachGiamGia = await _context.Saches
                .AsNoTracking()
                .Where(s => s.PhanTramGiamGia > 0)
                .ToListAsync();

            DateTime bestSellerPeriod = DateTime.Now.AddDays(-50);
            viewModel.SachBanChay = await _sachRepository.LaySachBanChay(days: 50, sachCount: 6);

            //Để tạm thời
            viewModel.TacGiaNoiTieng = await _context.TacGias.AsNoTracking().Take(9).ToListAsync(); 

            viewModel.TuSachMienPhi = await _context.Saches.AsNoTracking().Where(s => s.GiaBan == 0).Take(12).ToListAsync();

            //Lấy tạm thời
            var rawChuDes = await _context.ChuDes
                .AsNoTracking()
                .Where(cd => cd.ParentId == 1 || cd.ParentId == 2)
                .ToListAsync();

            var listChuDe = rawChuDes.Where(cd => cd.ParentId == 1).Take(4)
                .Concat(rawChuDes.Where(cd => cd.ParentId == 2).Take(5));

            foreach (ChuDe cd in listChuDe)
            {

                string duongDanHinhAnh = cd.TenChuDe switch
                {
                    "Văn Học" => "/img/product/van_hoc.jpg",
                    "Kinh Tế" => "/img/product/kinh_te.jpg",
                    "Tâm lý - Kĩ Năng Sống" => "/img/product/tam_ly.jpg",
                    "Sách Thiếu Nhi" => "/img/product/sach-thieu-nhi.jpg",
                    "Fiction" => "/img/product/fiction.jpg",
                    "Non-Fiction" => "/img/product/non_fiction.jpg",
                    "English Learning" => "/img/product/english.jpg",
                    "Children Books" => "/img/product/children_books.jpg",
                    _ => "/img/product/comic.jpg"
                };

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

                    string breadcrumbHtml = await _viewRenderService.RenderToStringAsync(
                        "_BreadcrumbPartial",
                        viewModel.Breadcrumbs
                    );

                    response["breadcrumb"] = breadcrumbHtml;
                }

                return Json(response);
            }

            return View("SachTheoChuDe", viewModel);
        }
        [HttpGet]
        public async Task<IActionResult> ChiTietSanPham(int maSach)
        {
            if (!_signInManager.IsSignedIn(User))
            {
                TempData["ReturnUrl"] = Url.Action("ChiTietSanPham", "Home", new { maSach = maSach });
            }

            var sach = await _context.Saches
                .Include(s => s.ChuDe)
                .Include(s => s.NhaXuatBan)
                .Include(s => s.TacGia)
                .Include(s => s.YeuThichs)
                .AsNoTracking()
                .FirstOrDefaultAsync(s => s.MaSach == maSach);

            ViewBag.UserDaThich = false;

            if (sach == null) return NotFound();

            // Code tải đánh giá để tính toán % sao
            var danhSachDanhGia = await _context.DanhGiaSanPhams
                .Where(d => d.MaSach == maSach)
                .AsNoTracking()
                .ToListAsync();

            int soLuongBan = await _context.ChiTietDonHangs
                .Where(ct => ct.MaSach == maSach && ct.DonHang.DaThanhToan == true)
                .SumAsync(ct => ct.SoLuong);

            var vm = new ChiTietSanPhamVM
            {
                Sach = sach,
                TongSoDanhGiaSanPham = danhSachDanhGia.Count,
                SoLuongSachBan = soLuongBan
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
                vm.DiemDanhGiaSanPhamTrungBinh = 5;
            }

            if (User.Identity.IsAuthenticated)
            {
                var user = await _userManager.GetUserAsync(User);
                if (user != null)
                {
                    var khachHang = await _context.KhachHangs.FirstOrDefaultAsync(k => k.MaTaiKhoan == user.Id);

                    if (khachHang != null)
                    {
                        bool daThich = await _context.YeuThichs.AnyAsync(y => y.MaSach == maSach && y.MaKhachHang == khachHang.MaKhachHang);
                        ViewBag.UserDaThich = daThich;
                    }
                }
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
                KhachHang khachHang = _context.KhachHangs.FirstOrDefault(kh => kh.MaTaiKhoan == userId);
                if (khachHang != null) maKhachHangHienTai = khachHang.MaKhachHang;
            }

            var danhSachDaThich = new HashSet<int>();
            if (maKhachHangHienTai.HasValue)
            {
                danhSachDaThich = _context.LuotThichDanhGiaSanPhams.Where(lt => lt.MaKhachHang == maKhachHangHienTai.Value)
                                                                   .Select(lt => lt.MaDanhGia).ToHashSet();
            }


            var query = _context.DanhGiaSanPhams.AsQueryable();
            query = query.Where(d => d.MaSach == maSach);

            if (kieuSapXep == "YeuThichNhat")
            {
                query = query.OrderByDescending(d => d.LuotThich).ThenByDescending(d => d.NgayDang);
            }
            else
            {
                query = query.OrderByDescending(d => d.NgayDang);
            }

            var danhSachDanhGia = await query
                                        .Include(d => d.KhachHang)
                                        .AsNoTracking()
                                        .ToListAsync();

            var danhGiaTheoTrang = danhSachDanhGia
                .Skip((trang - 1) * KICH_THUOC_TRANG)
                .Take(KICH_THUOC_TRANG)
                .ToList();

            ViewBag.DanhSachDaThich = danhSachDaThich;

            return PartialView("_DanhSachDanhGiaPartial", danhGiaTheoTrang);
        }

        // 3. [HttpPost] Xử lý Thích/Bỏ Thích
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ThichDanhGia(int maDanhGia)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var khachHang = await _context.KhachHangs.FirstOrDefaultAsync(k => k.MaTaiKhoan == userId);
            if (khachHang == null) return Unauthorized();

            var danhGia = await _context.DanhGiaSanPhams.FindAsync(maDanhGia);
            if (danhGia == null) return NotFound(); 

            var luotThichDaTonTai = await _context.LuotThichDanhGiaSanPhams
                .FirstOrDefaultAsync(l => l.MaKhachHang == khachHang.MaKhachHang && l.MaDanhGia == maDanhGia);

            bool daThich;
            if (luotThichDaTonTai != null)
            {
                _context.LuotThichDanhGiaSanPhams.Remove(luotThichDaTonTai);
                danhGia.LuotThich = Math.Max(0, danhGia.LuotThich - 1);
                daThich = false;
            }
            else
            {
                _context.LuotThichDanhGiaSanPhams.Add(new LuotThichDanhGiaSanPham
                {
                    MaKhachHang = khachHang.MaKhachHang,
                    MaDanhGia = maDanhGia
                });
                danhGia.LuotThich++;
                daThich = true;
            }

            _context.DanhGiaSanPhams.Update(danhGia);
            await _context.SaveChangesAsync();

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
        public async Task<IActionResult> VietDanhGiaPartialAsync(int maSach)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var khachHang = await _context.KhachHangs.FirstOrDefaultAsync(kh => kh.MaTaiKhoan == userId);

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
            var khachHang = _context.KhachHangs.FirstOrDefault(kh => kh.MaTaiKhoan == userId);

            if (!ModelState.IsValid)
            {
                return PartialView("_PopupVietDanhGiaSanPham", model);
            }

            model.MaKhachHang = khachHang.MaKhachHang;
            model.NgayDang = DateTime.UtcNow;

            _context.DanhGiaSanPhams.Add(model);
            await _context.SaveChangesAsync();

            return Json(new { success = true, message = "Gửi đánh giá thành công!" });
        }

        [HttpPost]
        public async Task<IActionResult> ToggleYeuThich(int maSach)
        {
            var maKhachHangClaim = User.FindFirstValue("MaKhachHang");
            if (string.IsNullOrEmpty(maKhachHangClaim))
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (userId == null) return Json(new { success = false, message = "Vui lòng đăng nhập!" });

                var khachHang = _context.KhachHangs.FirstOrDefault(k => k.MaTaiKhoan == userId);
                if (khachHang == null) return Json(new { success = false, message = "Lỗi tài khoản." });

                maKhachHangClaim = khachHang.MaKhachHang.ToString();
            }

            if (!int.TryParse(maKhachHangClaim, out int maKhachHang))
            {
                return Json(new { success = false, message = "Lỗi dữ liệu khách hàng." });
            }

            var yeuThich = _context.YeuThichs.FirstOrDefault(x => x.MaKhachHang == maKhachHang && x.MaSach == maSach);
            bool isLiked = false;

            if (yeuThich == null)
            {
                var newItem = new YeuThich { MaKhachHang = maKhachHang, MaSach = maSach };
                _context.YeuThichs.Add(newItem);
                isLiked = true;
            }
            else
            {
                _context.YeuThichs.Remove(yeuThich);
                isLiked = false;
            }

            await _context.SaveChangesAsync();

            int totalLikes = await _context.YeuThichs.CountAsync(x => x.MaSach == maSach);

            return Json(new { success = true, isLiked = isLiked, totalLikes = totalLikes });
        }

        [HttpGet]
        public IActionResult GetDanhSachNguoiThich(int maSach)
        {
            var listNguoiThich = _context.YeuThichs
                                    .Where(x => x.MaSach == maSach)
                                    .Include(x => x.KhachHang).Select(x => x.KhachHang).ToList();

            return PartialView("_ListNguoiThichPartial", listNguoiThich);
        }

        [HttpGet]
        public async Task<IActionResult> KiemTraTonKho(int maSach, int soLuongMuonTang)
        {
            var sach = await _context.Saches
                            .Include(s => s.ChuDe)
                            .Include(s => s.NhaXuatBan)
                            .Include(s => s.TacGia)
                            .AsNoTracking()
                            .FirstOrDefaultAsync(s => s.MaSach == maSach);
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

        public async Task<IActionResult> LaySachTheoTacGia(int id)
        {
            SachTheoChuDeVM viewModel = await LoadSachTheoChuDeVMAsync(0, 1, 12, null, new List<int> { id }, null, null, null);

            TacGia? selectedTacGia = await _context.TacGias.FirstOrDefaultAsync(tg => tg.MaTacGia == id);

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
            if (string.IsNullOrWhiteSpace(path) || path.Contains("tat-ca-chu-de"))
            {
                path = string.Empty;
            }
            else
            {
                path = Uri.UnescapeDataString(path);
            }

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

            viewModel.Breadcrumbs = await BuildBreadcrumbsAsync(selectedChuDe);
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

            SachTheoChuDeVM viewModel = new SachTheoChuDeVM();
            IQueryable<Sach> query = _context.Saches
                .Include(s => s.TacGia)
                .Include(s => s.NhaXuatBan)
                .Include(s => s.ChuDe)
                .AsNoTracking();

            // ===== LỌC THEO CHỦ ĐỀ =====
            if (chuDeId == 0) // Chủ đề là 0 thì tức là lấy tất cả chủ đề
            {
                viewModel.ChuDeCha = new ChuDe { Children = await _context.ChuDes.Where(c => c.ParentId == null).ToListAsync() };
                viewModel.ChuDeSelected = new ChuDe() { MaChuDe = 0};
            }
            else
            {
                ChuDe? selectedChuDe = await _context.ChuDes.FirstOrDefaultAsync(cd => cd.MaChuDe == chuDeId);
                viewModel.ChuDeCha = await LayChuDeTuongUng(selectedChuDe.MaChuDe);
                viewModel.ChuDeSelected = selectedChuDe;
                query = _sachRepository.LaySachTheoChuDe(chuDeId);
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
            else
            {
                viewModel.DanhSachTacGia = _context.TacGias.AsNoTracking();
                viewModel.DanhSachNhaXuatBan = _context.NhaXuatBans.AsNoTracking();
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

        private async Task<ChuDe?> LayChuDeTuongUng(int maChuDeDuocChon)
        {
            ChuDe? currentNode = await _context.ChuDes.FirstOrDefaultAsync(c => c.MaChuDe == maChuDeDuocChon);
            if (currentNode == null) return null;

            var children = _context.ChuDes.AsNoTracking().Where(cd => cd.ParentId == currentNode.MaChuDe);
            currentNode.Children = children.ToList();

            while (currentNode.ParentId != null)
            {
                var parentNode = await _context.ChuDes.FirstOrDefaultAsync(cd => cd.MaChuDe == currentNode.ParentId);
                if (parentNode == null) break;

                var siblings =  _context.ChuDes.AsNoTracking().Where(cd => cd.ParentId == parentNode.MaChuDe);
                parentNode.Children = siblings.ToList();

                var selectedIndex = parentNode.Children.FindIndex(cd => cd.MaChuDe == currentNode.MaChuDe);
                if(selectedIndex >= 0)
                {
                    parentNode.Children[selectedIndex] = currentNode;
                }
                currentNode = parentNode;
            }

            return currentNode;
        }

        private async Task<List<BreadcrumbItem>> BuildBreadcrumbsAsync(ChuDe chuDe)
        {
            var breadcrumbs = new List<BreadcrumbItem>
            {
                new BreadcrumbItem { Text = "Trang chủ", Url = "/" }
            };

            if(chuDe.MaChuDe == 0)
            {
                breadcrumbs.Add(new BreadcrumbItem { Text = "Tất cả chủ đề", Url = "/", IsActive = true });
                return breadcrumbs;
            }

            var pathSegments = new List<ChuDe>();
            var current = chuDe;

            while (current != null)
            {
                pathSegments.Insert(0, current);
                current = await _context.ChuDes.FirstOrDefaultAsync(cd => cd.MaChuDe == current.ParentId);
            }

            foreach (var segment in pathSegments)
            {
                breadcrumbs.Add(new BreadcrumbItem
                {
                    Text = segment.TenChuDe,
                    Url = "/chude/" + segment.FullPath,
                    IsActive = segment.MaChuDe == chuDe.MaChuDe
                });
            }

            return breadcrumbs;
        }
        #endregion



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

    }
}

