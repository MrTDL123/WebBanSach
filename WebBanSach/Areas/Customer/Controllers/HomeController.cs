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

namespace ProjectCuoiKi.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class HomeController : Controller
    {
        private readonly LocationService _locationService;
        private readonly IUnitOfWork _unit;
        public HomeController(UserManager<TaiKhoan> taiKhoan, IUnitOfWork unit, LocationService locationService)
        {
            _unit = unit;
            _locationService = locationService;
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

        public IActionResult Details(int MaSach)
        {
            Sach? sach = _unit.Saches.Get(u => u.MaSach == MaSach, includeProperties: "ChuDe,NhaXuatBan,TacGia");
            return View(sach);
        }

        #region Sách Theo Chủ Đề
        public async Task<IActionResult> LayTatCaSach(int? page)
        {
            int pageNumber = page ?? 1;
            ViewBag.Url = "Trang chủ > Tất cả chủ đề";


            SachTheoChuDeVM viewModel = new SachTheoChuDeVM();

            ChuDe allParentChuDe = new ChuDe();
            allParentChuDe.Children = await _unit.ChuDes.GetRangeReadOnlyAsync(cd => cd.ParentId == null);
            viewModel.ChuDeHienTai = allParentChuDe;
            List<Sach> danhSachSach = _unit.Saches.GetAll().ToList();
            viewModel.DanhSachSach = danhSachSach.ToPagedList(pageNumber, 20);
            viewModel.DanhSachTenNhaXuatBan = danhSachSach.Select(s => s.NhaXuatBan.TenNXB).Distinct();
            viewModel.DanhSachTenTacGia = danhSachSach.Select(s => s.TacGia.TenTG).Distinct();

            return View("SachTheoChuDe", viewModel);

        }

        public async Task<IActionResult> SachTheoChuDe(int? id, int? page, List<string> priceRanges)
        {                
            SachTheoChuDeVM viewModel = new SachTheoChuDeVM();
            int pageNumber = page ?? 1;
            ViewBag.Url = LayURL(id, new List<string>());

            viewModel.PriceFilter = new PriceFilter()
            {
                PriceRanges = LoadPriceRanges(),
                SelectedRanges = priceRanges ?? new List<string>()
            };

            ChuDe selectedChuDe = _unit.ChuDes.Get(cd => cd.MaChuDe == id);

            viewModel.ChuDeHienTai = await LayChuDeLevel1(selectedChuDe);
            List<Sach>? danhSachSach = await _unit.Saches.LaySachTheoChuDe(id);

            if (!priceRanges.IsNullOrEmpty() && priceRanges.Count > 0)
            {
                danhSachSach = GetPriceFilteredSaches(priceRanges, danhSachSach);
            }



            viewModel.DanhSachSach = danhSachSach.ToPagedList(pageNumber, 20);
            viewModel.DanhSachTenNhaXuatBan = danhSachSach.Select(s => s.NhaXuatBan.TenNXB).Distinct();
            viewModel.DanhSachTenTacGia = danhSachSach.Select(s => s.TacGia.TenTG).Distinct();


            return View(viewModel);
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

        private List<Sach> GetPriceFilteredSaches(List<string> priceRangesId, List<Sach> danhSach)
        {

            if(priceRangesId != null && priceRangesId.Any())
            {
                List<PriceRange> ranges = LoadPriceRanges()
                                    .Where(r => priceRangesId.Contains(r.Id))
                                    .ToList();

                danhSach = danhSach.Where(s => ranges.Any(r =>
                    s.GiaBan >= r.MinPrice &&
                    (r.MaxPrice == null || s.GiaBan <= r.MaxPrice))).ToList();
            }

            return danhSach;
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

#endregion