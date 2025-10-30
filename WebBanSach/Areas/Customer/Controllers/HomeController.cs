using Media.DataAccess.Repository.IRepository;
using Media.Models;
using Media.Models.ViewModels;
using Media.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;
using PagedList;
using System.Text;

namespace ProjectCuoiKi.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class HomeController : Controller
    {
        private readonly IUnitOfWork _unit;
        public HomeController(ILogger<HomeController> logger, IUnitOfWork unit, LocationService locationService)
        public HomeController(UserManager<TaiKhoan> taiKhoan, IUnitOfWork unit)
        {
            _unit = unit;
            _locationService = locationService;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult SachBanNhieu() //Phải chỉnh để lấy sách có số lượng bán nhiều
        {
            //IndexVM category_productlist = new()
            //{
            //    DanhSachChuDe = _unit.ChuDes.GetAll(),
            //    DanhSachSanPham = _unit.Saches.GetAll(includeProperties: "TacGia")
            //};

            //return View(category_productlist);
            return View();
        }

        public IActionResult Details(int id)
        {
            Sach? product = _unit.Saches.Get(u => u.MaSach == id ,includeProperties: "ChuDe");
            return View(product);
        }

        public async Task<IActionResult> SachTheoChuDe(int id, int? page)
        {
            int pageNumber = page ?? 1; 
            List<Sach> list = await _unit.Saches.LaySachTheoChuDe(id);

            ViewBag.Url = LayURL(id, new List<string>());
            return View(list.ToPagedList(pageNumber, 20));
        }
         
        private string LayURL(int? maCD, List<string> url)
        {
            ChuDe selectedChuDe = _unit.ChuDes.Get(cd => cd.MaChuDe == maCD);

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

            var model = new Checkout
            {
                ProductName = "Bộ Manga - Summer Ghost - Bóng Ma Mùa Hạ - Tập 1 + Tập 2",
                ProductImage = "/images/product.jpg",
                Quantity = 1,
                SubTotal = 76000,
                ShippingFee = 20000,
                Total = 96000,

                Countries = new List<SelectListItem>
                {
                    new SelectListItem { Value = "VN", Text = "Việt Nam", Selected = true }
                },
                Provinces = provinces, // Lấy từ API thật
                Districts = new List<SelectListItem>
                {
                    new SelectListItem { Value = "", Text = "Chọn quận/huyện", Disabled = true, Selected = true }
                },
                Wards = new List<SelectListItem>
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

            var model = new Checkout
            {
                ProductName = "Bộ Manga - Summer Ghost - Bóng Ma Mùa Hạ - Tập 1 + Tập 2",
                ProductImage = "/images/product.jpg",
                Quantity = 1,
                SubTotal = 76000,
                ShippingFee = 20000,
                Total = 96000,

                Countries = new List<SelectListItem>
                {
                    new SelectListItem { Value = "VN", Text = "Việt Nam", Selected = true }
                },
                Provinces = provinces, // Lấy từ API thật
                Districts = new List<SelectListItem>
                {
                    new SelectListItem { Value = "", Text = "Chọn quận/huyện", Disabled = true, Selected = true }
                },
                Wards = new List<SelectListItem>
                {
                    new SelectListItem { Value = "", Text = "Chọn phường/xã", Disabled = true, Selected = true }
                }
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ThanhToan(Checkout model)
        {
            if (!ModelState.IsValid)
            {
                // Nếu fail, load lại dữ liệu dropdown từ API
                model.Provinces = await _locationService.GetProvincesAsync();
                model.Districts = new List<SelectListItem>();
                model.Wards = new List<SelectListItem>();
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
    }
}