using Media.DataAccess.Repository.IRepository;
using Media.Models;
using Media.Models.ViewModels;
using Media.Service;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using X.PagedList;
using X.PagedList.Extensions;
using Microsoft.IdentityModel.Tokens;
using Media.Service.IServices;

namespace ProjectCuoiKi.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class HomeController : Controller
    {

        private readonly LocationService _locationService;
        private readonly IViewRenderService _viewRenderService;
        private readonly IUnitOfWork _unit;
        public HomeController(UserManager<TaiKhoan> taiKhoan, IUnitOfWork unit, LocationService locationService, IViewRenderService viewRenderService)
        {
            _unit = unit;
            _locationService = locationService;
            _viewRenderService = viewRenderService;
        }

        public IActionResult Index()
        {
            IndexVM viewModel = new IndexVM()
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

        public IActionResult Details(int id)
        {
            Sach? sach = _unit.Saches.Get(u => u.MaSach == id, includeProperties: "ChuDe,NhaXuatBan,TacGia");
            return View(sach);
        }

        #region Sách Theo Chủ Đề
        public async Task<IActionResult> LayTatCaSach(int? page)
        {
            int pageNumber = page ?? 1;
            ViewBag.Url = "Trang chủ > Tất cả chủ đề";

            SachTheoChuDeVM viewModel = await LoadSachTheoChuDeVMAsync(0, pageNumber, null, null, null, null);

            viewModel.DanhSachTacGia = await _unit.TacGias.GetAllReadOnlyAsync();
            viewModel.DanhSachNhaXuatBan = await _unit.NhaXuatBans.GetAllReadOnlyAsync();

            return View("SachTheoChuDe", viewModel);
        }

        public async Task<IActionResult> SachTheoChuDe(
            int id, int? page,
            List<string> priceRanges, 
            List<int> tacGiaIds,
            List<int> nhaXuatBanIds,
            string keyword)
        {
            int pageNumber = page ?? 1;
            ViewBag.Url = LayURL(id, new List<string>());

            SachTheoChuDeVM viewModel = await LoadSachTheoChuDeVMAsync(id, pageNumber, priceRanges, tacGiaIds, nhaXuatBanIds, keyword);

            bool isAjax = Request.Headers["X-Requested-With"] == "XMLHttpRequest";

            if (isAjax)
            {
                string chuDeTuongUngHtml = await _viewRenderService.RenderToStringAsync("_ChuDeTuongUngPartial", viewModel.ChuDeHienTai);
                string sachListHtml = await _viewRenderService.RenderToStringAsync("_SachListPartial", viewModel.DanhSachSach);
                string danhSachTacGia = await _viewRenderService.RenderToStringAsync("_TacGiaFilterPartial", viewModel.DanhSachTacGia);
                string danhSachNhaXuatBan = await _viewRenderService.RenderToStringAsync("_NhaXuatBanFilterPartial", viewModel.DanhSachNhaXuatBan);
                return Json(new
                {
                    success = true,
                    chuDeTuongUng = chuDeTuongUngHtml,
                    sachList = sachListHtml,
                    tacGiaList = danhSachTacGia,
                    nhaXuatBanList = danhSachNhaXuatBan
                });
            }
            return View(viewModel);
        }

        private async Task<SachTheoChuDeVM> LoadSachTheoChuDeVMAsync(
            int chuDeId, int page,
            List<string>? priceRanges,
            List<int>? tacGiaIds,
            List<int>? nhaXuatBanIds,
            string? keyword)
        {
            IQueryable<Sach> query;

            SachTheoChuDeVM viewModel = new SachTheoChuDeVM();

            // ===== LỌC THEO CHỦ ĐỀ =====
            if (chuDeId == 0) // Chủ đề là 0 thì tức là lấy tất cả chủ đề
            {
                ChuDe allParentChuDe = new ChuDe();
                allParentChuDe.Children = await _unit.ChuDes.GetRangeReadOnlyAsync(cd => cd.ParentId == null);
                viewModel.ChuDeHienTai = allParentChuDe;

                query = _unit.Saches.GetAll().AsQueryable()
                    .Include(s => s.TacGia)
                    .Include(s => s.NhaXuatBan)
                    .Include(s => s.ChuDe);
            }
            else
            {
                ChuDe? selectedChuDe = _unit.ChuDes.Get(cd => cd.MaChuDe == chuDeId);
                viewModel.ChuDeHienTai = await LayChuDeLevel1(selectedChuDe);

                query = _unit.Saches.LaySachTheoChuDe(chuDeId);
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


            // ===== LỌC THEO TỪ KHÓA =====
            if (!string.IsNullOrWhiteSpace(keyword))
            {
                query = query.Where(s => s.TenSach.Contains(keyword));
            }


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


            viewModel.DanhSachSach = sachList.ToPagedList(page, 20);
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

        private async Task<ChuDe?> LayChuDeLevel1(ChuDe selectedChuDe)
        {
            ChuDe? parentChuDe = _unit.ChuDes.Get(cd => cd.MaChuDe == selectedChuDe.ParentId);

            selectedChuDe.Children = await _unit.ChuDes.GetRangeReadOnlyAsync(cd => cd.ParentId == selectedChuDe.MaChuDe);
            if(parentChuDe is null)
            {
                return selectedChuDe;
            }

            parentChuDe.Children = new List<ChuDe>() { selectedChuDe };

            return await LayChuDeLevel1(parentChuDe);
        }


        private string LayURL(int? maCD, List<string> url)
        {
            ChuDe? selectedChuDe = _unit.ChuDes.Get(cd => cd.MaChuDe == maCD);
            if(selectedChuDe is null)
            {
                return "Trang chủ > Tất cả chủ đề";
            }

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
                MienPhiVanChuyen = 20000,
                TongTien = 96000,

                list_TinhThanh = provinces, // Lấy từ API thật
                list_QuanHuyen = new List<SelectListItem>
                {
                    new SelectListItem { Value = "", Text = "Chọn quận/huyện", Disabled = true, Selected = true }
                },
                list_PhuongXa = new List<SelectListItem>
                {
                    new SelectListItem { Value = "", Text = "Chọn phường/xã", Disabled = true, Selected = true }
                }
            };
            return PartialView("_PopupThayDoiDiaChiGiaoHang", model);
        }

        public IActionResult WriteReviewPartial()
        {
            return PartialView("_PopupVietDanhGiaSanPham");
        }

        public IActionResult ShopingCart()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> ThanhToan()
        {
            var provinces = await _locationService.GetProvincesAsync();

            var model = new ThanhToan
            {
                TenSanPham = "Bộ Manga - Summer Ghost - Bóng Ma Mùa Hạ - Tập 1 + Tập 2",
                HinhAnhSanPham = "/images/product.jpg",
                SoLuong = 1,
                TamTinh = 76000,
                MienPhiVanChuyen = 20000,
                TongTien = 96000,

                list_TinhThanh = provinces, // Lấy từ API thật
                list_QuanHuyen = new List<SelectListItem>
                {
                    new SelectListItem { Value = "", Text = "Chọn quận/huyện", Disabled = true, Selected = true }
                },
                list_PhuongXa = new List<SelectListItem>
                {
                    new SelectListItem { Value = "", Text = "Chọn phường/xã", Disabled = true, Selected = true }
                }
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ThanhToan(ThanhToan model)
        {
            if (!ModelState.IsValid)
            {
                // Nếu fail, load lại dữ liệu dropdown từ API
                model.list_TinhThanh = await _locationService.GetProvincesAsync();
                model.list_QuanHuyen = new List<SelectListItem>();
                model.list_PhuongXa = new List<SelectListItem>();
                return View(model);
            }

            // Xử lý thanh toán...
            return RedirectToAction("OrderSuccess");
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


        public IActionResult OrderSuccess()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> ThongTinCaNhan()
        {
            var provinces = await _locationService.GetProvincesAsync();

            var model = new ThanhToan
            {
                //ProductName = "Bộ Manga - Summer Ghost - Bóng Ma Mùa Hạ - Tập 1 + Tập 2",
                //ProductImage = "/images/product.jpg",
                //Quantity = 1,
                //SubTotal = 76000,
                //ShippingFee = 20000,
                //Total = 96000,

                list_TinhThanh = provinces, // Lấy từ API thật
                list_QuanHuyen = new List<SelectListItem>
                {
                    new SelectListItem { Value = "", Text = "Chọn quận/huyện", Disabled = true, Selected = true }
                },
                list_PhuongXa = new List<SelectListItem>
                {
                    new SelectListItem { Value = "", Text = "Chọn phường/xã", Disabled = true, Selected = true }
                }
            };

            return View(model);
        }


    }
}

