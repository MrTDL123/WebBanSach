using Media.Models; // Namespace chứa Enum TrangThaiGiaoHang

namespace Media.Models.ViewModels
{
    public class ChiTietDonHangVM
    {
        public int MaDonHang { get; set; } // Model dùng int
        public DateTime NgayDat { get; set; }
        public TrangThaiGiaoHang TrangThai { get; set; } // Map từ VanChuyen sang

        public string NguoiNhan { get; set; }
        public string SoDienThoai { get; set; }
        public string DiaChiGiaoHang { get; set; } // Sẽ gộp chuỗi ở Controller

        public string HinhThucThanhToan { get; set; }
        public bool DaThanhToan { get; set; }

        public decimal Total { get; set; } // Tổng bill (Model là Total)
        // Trong Model DonHang không thấy phí ship, nếu có trong VanChuyen thì map vào, tạm để 0
        public decimal PhiVanChuyen { get; set; }

        public List<ChiTietSachVM> ChiTietSachs { get; set; }
    }

    public class ChiTietSachVM
    {
        public int MaSach { get; set; }
        public string TenSach { get; set; }
        public string HinhAnh { get; set; }
        public int SoLuong { get; set; }
        public decimal DonGia { get; set; }
        public decimal ThanhTien { get; set; }
    }
}