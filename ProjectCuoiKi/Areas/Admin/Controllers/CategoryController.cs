using Microsoft.AspNetCore.Mvc;
using Media.Models;
using Meida.DataAccess.Data;
using Media.DataAccess.Repository.IRepository;
using Media.DataAccess.Repository;
using Microsoft.AspNetCore.Authorization;
using Media.Utility;

namespace ProjectCuoiKi.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
    public class CategoryController : Controller
    {
        private readonly IUnitOfWork _unit;
        public CategoryController(IUnitOfWork unitOfWork)
        {
            _unit = unitOfWork;
        }
        public IActionResult Index()
        {
            List<Category> CategoryList = _unit.Category.GetAll().ToList();
            return View(CategoryList);
        }
        public IActionResult Upsert(int? id)
        {
            if(id == null || id == 0)
            {
                Category obj = new Category();
                return View(obj);
            }
            else
            {
                Category? objFromDb = _unit.Category.Get(u => u.Id == id);
                return View(objFromDb); 
            }
            
        }
        [HttpPost]
        public IActionResult Upsert(Category obj)
        {
            if (obj.Name == obj.DisplayOrder.ToString()) //Custom Validation
            {
                ModelState.AddModelError("Name", "Tên và Số thứ tự không được giống nhau");
            }
            if (ModelState.IsValid)
            {
                if(obj.Id == 0)
                {
                    _unit.Category.Add(obj);
                    _unit.Save();
                    TempData["success"] = "Đã thêm thể loại mới";
                }
                else
                {
                    _unit.Category.Update(obj);
                    _unit.Save();
                    TempData["success"] = "Đã cập nhập thể loại thành công";
                }
                return RedirectToAction("Index");
            }
            return View();
        }


        public IActionResult Delete(int? id)
        {
            Category? categoryFromDb = _unit.Category.Get(u => u.Id == id);
            if (categoryFromDb == null)
            {
                TempData["error"] = "Thể loại không tồn tại";
                return RedirectToAction("Index");
            }
            _unit.Category.Remove(categoryFromDb);
            _unit.Save();
            TempData["success"] = "Đã xóa thể loại";
            return RedirectToAction("Index");
        }
    }
}
