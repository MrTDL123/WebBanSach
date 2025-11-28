using Media.Models;
using Media.Utility;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Media.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class AdminController : Controller
    {
        private readonly SignInManager<TaiKhoan> _signInManager;
        private readonly UserManager<TaiKhoan> _userManager;

        public AdminController(SignInManager<TaiKhoan> signInManager,
                               UserManager<TaiKhoan> userManager)
        {
            _signInManager = signInManager;
            _userManager = userManager;
        }

        // GET: Admin/Admin/DangNhap
        [HttpGet]
        public IActionResult DangNhap() => View();

        [HttpPost]
        public async Task<IActionResult> DangNhap(string email, string matKhau)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user != null && await _userManager.CheckPasswordAsync(user, matKhau))
            {
                if (await _userManager.IsInRoleAsync(user, SD.Role_Admin) ||
                    await _userManager.IsInRoleAsync(user, SD.Role_Employee))
                {
                    await _signInManager.SignInAsync(user, false);
                    return RedirectToAction("Index", "Dashboard", new { area = "Admin" });
                }
                ModelState.AddModelError("", "Bạn không có quyền truy cập vào trang quản trị.");
            }
            ModelState.AddModelError("", "Đăng nhập thất bại, vui lòng kiểm tra lại.");
            return View();
        }

        public async Task<IActionResult> DangXuat()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("DangNhap", "Admin", new { area = "Admin" });
        }
    }
}