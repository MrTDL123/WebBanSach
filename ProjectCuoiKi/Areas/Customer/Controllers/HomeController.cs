using Microsoft.AspNetCore.Mvc;
using Media.Models;
using System.Diagnostics;
using Media.DataAccess.Repository.IRepository;
using Media.Models.ViewModels;

namespace ProjectCuoiKi.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IUnitOfWork _unit;
        public HomeController(ILogger<HomeController> logger, IUnitOfWork unit)
        {
            _logger = logger;
            _unit = unit;
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
        public IActionResult Shop()
        {
            IEnumerable<Product> productList = _unit.Product.GetAll(includeProperties: "Category");
            return View(productList);
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
    }
}
