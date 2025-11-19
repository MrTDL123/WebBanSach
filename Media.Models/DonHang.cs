using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Media.Models
{
    public enum HinhThucThanhToan
    {
        [Description("Thanh toán tiền mặt khi nhận hàng")]
        TienMatKhiNhanHang,
        [Description("Chuyển khoản")]
        ChuyenKhoan
    }

    public class DonHang
    {
        [Key]
        public int MaDonHang { get; set; }
        [Required]
        public int MaKhachHang { get; set; }
        [Required]
        public int MaNhanVien { get; set; }
        [Required]
        public int MaVanChuyen { get; set; }
        [Required]
        public int? MaDiaChi { get; set; }
        [Required, StringLength(100)]
        [DisplayName("Tên người nhận")]
        public string TenNguoiNhan { get; set; }

        [Required, StringLength(20)]
        [DisplayName("Số điện thoại người nhận")]
        public string SoDienThoaiNhan { get; set; }
        [Required, StringLength(50)]
        [DisplayName("Phường/Xã")]
        public string PhuongXa { get; set; }

        [Required, StringLength(50)]
        [DisplayName("Quận/Huyện")]
        public string QuanHuyen { get; set; }

        [Required, StringLength(50)]
        [DisplayName("Tỉnh/Thành phố")]
        public string TinhThanh { get; set; }

        [Required, StringLength(200)]
        [DisplayName("Địa chỉ giao hàng chi tiết")]
        public string DiaChiChiTiet { get; set; }

        [StringLength(200)]
        [DisplayName("Ghi chú")]
        public string? GhiChu { get; set; }

        [Column(TypeName = "decimal(18,2)"), DisplayName("Tông tiền")]
        public decimal Total { get; set; }
        [Required, DisplayName("Ngày tạo"), DataType(DataType.Date)]
        public DateTime NgayTao { get; set; } = DateTime.UtcNow;
        [DataType(DataType.Date), DisplayName("Ngày cập nhật")]
        public DateTime? NgayCapNhat { get; set; }
        [Required]
        public bool DaThanhToan { get; set; }
        [Required]
        [DisplayName("Hình thức thanh toán")]
        public HinhThucThanhToan HinhThucThanhToan { get; set; } = HinhThucThanhToan.TienMatKhiNhanHang;
        //NAVIGATION PROPERTIES
        [ForeignKey("MaKhachHang")]
        public KhachHang KhachHang { get; set; }
        [ForeignKey("MaNhanVien")]
        public NhanVien NhanVien { get; set; }
        [ForeignKey(nameof(MaVanChuyen))]
        public VanChuyen VanChuyen { get; set; }
        [ForeignKey(nameof(MaDiaChi))]
        public DiaChiNhanHang DiaChiNhanHang { get; set; }
        
        public ICollection<ChiTietDonHang> ChiTietDonHangs { get; set; }
        public ICollection<PhieuTraHang> PhieuTraHangs { get; set; }
        public HoaDon HoaDon { get; set; }
    }
}