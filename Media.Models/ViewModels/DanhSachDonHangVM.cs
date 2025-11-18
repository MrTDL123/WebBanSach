using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Media.Models.ViewModels
{
    public class DanhSachDonHangVM
    {
        public int MaDonHang { get; set; }
        public TrangThaiGiaoHang TrangThai { get; set; }
        public decimal TongTien { get; set; }
        public DateTime NgayDat { get; set; }

        // --- NÂNG CẤP PHẦN SẢN PHẨM ---
        public string TenSanPhamDauTien { get; set; }
        public string HinhAnhDauTien { get; set; }

        // Số lượng của riêng sản phẩm đầu tiên
        public int SoLuongSanPhamDauTien { get; set; }

        // Số lượng CÁC LOẠI sản phẩm khác (ví dụ: mua Sách A, B, C thì số này = 2)
        public int SoLuongSanPhamKhac { get; set; }
    }
}