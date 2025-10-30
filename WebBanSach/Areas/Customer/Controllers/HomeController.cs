using Media.DataAccess.Repository.IRepository;
using Media.Models;
using Media.Models.ViewModels;
using Media.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace ProjectCuoiKi.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class HomeController : Controller
    {
        private readonly LocationService _locationService;
        private readonly ILogger<HomeController> _logger;
        private readonly IUnitOfWork _unit;
        public HomeController(ILogger<HomeController> logger, IUnitOfWork unit, LocationService locationService)
        {
            _logger = logger;
            _unit = unit;
            _locationService = locationService;
        }

        public IActionResult Index()
        {
            IndexVM category_productlist = new()
            {
                categoryList = _unit.Category.GetAll().ToList(),
                products = _unit.Product.GetAll(includeProperties: "Category")
            };
            return View(category_productlist);
        }

        public IActionResult Details(int id)
        {
            Product product = _unit.Product.Get(u => u.Id == id ,includeProperties: "Category");
            return View(product);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
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