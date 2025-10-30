using Microsoft.AspNetCore.Mvc;
using Media.Models;
using System.Diagnostics;
using Media.DataAccess.Repository.IRepository;
using Media.Models.ViewModels;
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
    }
}
