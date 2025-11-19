using Microsoft.AspNetCore.Mvc;
using Media.Models.ViewModels;
using Media.Utility;

namespace WebBanSach.ViewComponents
{
    public class GioHangViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            // Lấy danh sách giỏ hàng từ Session
            var gioHang = HttpContext.Session.GetObjectFromJson<List<GioHangVM>>("GioHang") ?? new List<GioHangVM>();

            // Tính tổng số lượng sản phẩm
            int tongSoLuong = gioHang.Count();

            // Trả về view kèm dữ liệu
            return View(tongSoLuong);
        }
    }
}