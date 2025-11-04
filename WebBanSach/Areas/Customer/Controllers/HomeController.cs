using Microsoft.AspNetCore.Mvc;
using Media.Models;
using System.Diagnostics;
using Media.DataAccess.Repository.IRepository;
using Media.Models.ViewModels;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;
using X.PagedList;
using X.PagedList.Extensions;

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
        #endregion
    }
}
