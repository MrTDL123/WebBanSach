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
    public class ChuDeController : Controller
    {
        private readonly IUnitOfWork _unit;
        public ChuDeController(IUnitOfWork unitOfWork)
        {
            _unit = unitOfWork;
        }
        public IActionResult Index()
        {
            //List<ChuDe> ChuDeList = _unit.Category.GetAll().ToList();
            //return View(ChuDeList);
            return View();
        }
        public IActionResult Upsert(int? id)
        {
            if(id == null)
            {
                ChuDe obj = new ChuDe();
                return View(obj);
            }
            else
            {
                ChuDe? objFromDb = _unit.ChuDes.Get(u => u.MaChuDe == id);
                return View(objFromDb); 
            }
            
        }
        [HttpPost]
        public IActionResult Upsert(ChuDe obj)
        {
            if (ModelState.IsValid)
            {
                if(obj.MaChuDe == null)
                {
                    _unit.ChuDes.Add(obj);
                    _unit.Save();
                    TempData["success"] = "Đã thêm thể loại mới";
                }
                else
                {
                    _unit.ChuDes.Update(obj);
                    _unit.Save();
                    TempData["success"] = "Đã cập nhập thể loại thành công";
                }
                return RedirectToAction("Index");
            }
            return View();
        }


        public IActionResult Delete(int? id)
        {
            ChuDe? ChuDeFromDb = _unit.ChuDes.Get(u => u.MaChuDe == id);
            if (ChuDeFromDb == null)
            {
                TempData["error"] = "Thể loại không tồn tại";
                return RedirectToAction("Index");
            }
            _unit.ChuDes.Remove(ChuDeFromDb);
            _unit.Save();
            TempData["success"] = "Đã xóa thể loại";
            return RedirectToAction("Index");
        }
    }
}
