using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Media.Models.ViewModels;

namespace Media.Service
{
    public interface IGioHangService
    {
        // Hàm tải giỏ hàng từ DB
        List<GioHangVM> TaiGioHangTuDb(string userId);

        // Hàm lưu giỏ hàng vào DB
        void LuuGioHangVaoDb(string userId, List<GioHangVM> gioHang);

        // Hàm gộp giỏ hàng
        List<GioHangVM> GopGioHangSessionVaoDb(string userId);
    }
}