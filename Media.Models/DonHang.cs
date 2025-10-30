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
        public int MaDonHang { get; set; }
        [Required]
        public bool DaThanhToan { get; set; }
        [Required]
        public TinhTrangGiaoHang TinhTrangGiaoHang { get; set; } = TinhTrangGiaoHang.ChoXacNhan;
        [Required]
        public DateTime NgayDat {  get; set; } = DateTime.Now;
        [Required]
        public DateTime NgayGiao { get; set; }

        //Thuộc tính chỉ dành cho Hậu Cần
        public DateTime? NgayTaoDonVanChuyen { get; set; }
        public DateTime? NgayGuiHangVanChuyen { get; set; }



        //NAVIGATION PROPERTIES
        public int MaKhachHang { get; set; }

        [ForeignKey("MaKhachHang")]
        public KhachHang KhachHang { get; set; }
        public int MaNhanVien { get; set; }
        [ForeignKey("MaNhanVien")]
        public NhanVien NhanVien { get; set; }

        public ICollection<ChiTietDonHang> ChiTietDonHangs { get; set; }
        public HoaDon HoaDon { get; set; }

    }
}
