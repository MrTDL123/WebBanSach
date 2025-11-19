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
    public enum TrangThaiGiaoHang
    {
        [Description("Chờ xử lý")]
        ChoXuLy,
        [Description("Đang giao hàng")]
        DangGiaoHang,
        [Description("Giao hàng thành công")]
        GiaoHangThanhCong,
        [Description("Giao thất bại")]
        GiaoThatBai,
        [Description("Đã hủy")]
        DaHuy
    }

    public class VanChuyen
    {
        [Key]
        public int MaDonHang { get; set; }
        [Required, MaxLength(100)]
        [DisplayName("Đơn vị vận chuyển")]
        public string DonViVanChuyen { get; set; }
        [Required, MaxLength(50)]
        [DisplayName("Mã vận đơn")]
        public string MaVanDon { get; set; }
        [DisplayName("Phí vận chuyển")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal PhiVanChuyen { get; set; }
        [DisplayName("Trạng thái giao hàng")]
        public TrangThaiGiaoHang TrangThaiGiaoHang { get; set; } = TrangThaiGiaoHang.ChoXuLy;
        [DataType(DataType.Date)]
        [DisplayName("Ngày dự kiến giao")]
        public DateTime? NgayDuKienGiao { get; set; }

        [DataType(DataType.Date)]
        [DisplayName("Ngày nhận hàng thực tế")]
        public DateTime? NgayNhanHangThucTe { get; set; }

        public DonHang DonHang { get; set; }
    }
}