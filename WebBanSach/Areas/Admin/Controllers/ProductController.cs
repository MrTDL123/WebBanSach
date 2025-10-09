using Media.DataAccess.Repository.IRepository;
using Media.Models;
using Media.Models.ViewModels;
using Media.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ProjectCuoiKi.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]

    public class ProductController : Controller
    {
        private readonly IUnitOfWork _unit;
        private readonly IWebHostEnvironment _webHostEnvironment;//Dùng để truy cập vào các folder
        public ProductController(IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment)
        {
            _unit = unitOfWork;
            _webHostEnvironment = webHostEnvironment;
        }
        public IActionResult Index()
        {
            List<Product> ProductList = _unit.Product.GetAll(includeProperties:"Category").ToList();

            return View(ProductList);
        }
        public IActionResult Upsert(int? id)
        {
            ProductVM productVM = new()
            {
                Product = new Product(),
                CategoryList = _unit.Category.GetAll().Select(u => new SelectListItem
                {
                    Text = u.Name,
                    Value = u.Id.ToString()
                })
            };
            if(id == null || id ==0)
            {
                //Create
                return View(productVM);
            }
            else
            {
                //Update
                productVM.Product = _unit.Product.Get(u => u.Id == id);
                return View(productVM);
            }
        }
        [HttpPost]
        public IActionResult Upsert(ProductVM obj, IFormFile? file)
        {
            if (ModelState.IsValid)
            {
                string wwwRootPath = _webHostEnvironment.WebRootPath;
                if(file != null)
                {
                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);//Tên file
                    string productPath = Path.Combine(wwwRootPath, @"img\product");//Đường dẫn để lưu file
                    if(!string.IsNullOrEmpty(obj.Product.ImageUrl))
                    {
                        //Xóa ảnh cũ
                        var oldImagePath = Path.Combine(wwwRootPath, obj.Product.ImageUrl.TrimStart('\\'));
                        if (System.IO.File.Exists(oldImagePath))
                        {
                            System.IO.File.Delete(oldImagePath);
                        }
                    }
                    using (var fileStream = new FileStream(Path.Combine(productPath, fileName), FileMode.Create))
                    {
                        file.CopyTo(fileStream);
                    }
                    obj.Product.ImageUrl = @"\img\product\" + fileName;
                }
                if (obj.Product.Id == 0)
                {
                    //Tạo
                     _unit.Product.Add(obj.Product);
                }
                else
                {
                    //Cập nhập
                    _unit.Product.Update(obj.Product);
                }
                _unit.Save();
                TempData["success"] = "Đã thêm sản phẩm mới";
                return RedirectToAction("Index");
            }
            else
            {
                obj.CategoryList = _unit.Category.GetAll().Select(u => new SelectListItem
                {
                    Text = u.Name,
                    Value = u.Id.ToString()
                });
                TempData["error"] = "Hãy kiểm tra lại các thông tin đã điền";
                return View(obj);
            }
        }



        #region API CALLS
        [HttpGet]
        public IActionResult GetAll()
        {
            List<Product> ProductList = _unit.Product.GetAll(includeProperties: "Category").ToList();
            return Json(new { data = ProductList });//lấy API endpoint là trang Json để có thể lấy được dữ liệu
        }

        [HttpDelete]
        public IActionResult Delete(int? id)
        {
            var productToBeDeleted = _unit.Product.Get(u => u.Id == id);
            if(productToBeDeleted == null)
            {
                return Json(new {success = false, message = "Không thể xóa sản phẩm không tồn tại"});
            }
            var oldImagePath = Path.Combine(_webHostEnvironment.WebRootPath, productToBeDeleted.ImageUrl.TrimStart('\\'));
            if (System.IO.File.Exists(oldImagePath))
            {
                System.IO.File.Delete(oldImagePath);
            }
            _unit.Product.Remove(productToBeDeleted);
            _unit.Save();
            return Json(new { success = true, message = "Xóa thành công" });
        }
        #endregion



    }
}
