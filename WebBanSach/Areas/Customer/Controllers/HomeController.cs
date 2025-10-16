using Microsoft.AspNetCore.Mvc;
using Media.Models;
using System.Diagnostics;
using Media.DataAccess.Repository.IRepository;
using Media.Models.ViewModels;
using Microsoft.AspNetCore.Identity;

namespace ProjectCuoiKi.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class HomeController : Controller
    {
        private readonly IUnitOfWork _unit;
        public HomeController(UserManager<TaiKhoan> taiKhoan, IUnitOfWork unit)
        {
            _unit = unit;
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
            Sach? product = _unit.Saches.Get(u => u.Id == id ,includeProperties: "ChuDe");
            return View(product);
        }
    }
}
