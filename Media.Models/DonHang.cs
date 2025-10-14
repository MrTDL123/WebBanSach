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
    public enum TinhTrangGiaoHang
    {
        [Description("Chờ xác nhận")]
        ChoXacNhan,
        [Description("Đã xác nhận")]
        DaXacNhan,
        [Description("Chờ lấy hàng")]
        ChoLayHang,
        [Description("Đang giao")]
        DangGiao,
        [Description("Hàng hóa gặp sự cố")]
        GapSuCo,
        [Description("Đã giao thành công")]
        ThanhCong,
        [Description("Giao hàng thất bại")]
        ThatBai,
        [Description("Đã hủy")]
        DaHuy,
        [Description("Trả hàng")]
        TraHang
    }
    public class DonHang
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public bool DaThanhToan { get; set; }
        [Required]
        public TinhTrangGiaoHang TinhTrangGiaoHang { get; set; } = TinhTrangGiaoHang.ChoXacNhan;
        [Required]
        public DateTime NgayDat {  get; set; } = DateTime.Now;
        [Required]
        public DateTime NgayGiao { get; set; }

        //NAVIGATION PROPERTIES
        public int KhachHangId { get; set; }
        public int ChiTietDonHangId { get; set; }

        [ForeignKey("KhachHangId")]
        public TaiKhoan KhachHang { get; set; }
        [ForeignKey("ChiTietDonHangId")]
        public ChiTietDonHang ChiTietDonHang { get; set; }
    }
}
