using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Media.Models
{
    public class HoaDon
    {
        [Key]
        [DisplayName("Mã hóa đơn")]
        public int MaHoaDon { get; set; }

        [DataType(DataType.Date)]
        [DisplayName("Ngày xuất")]
        public DateTime? NgayXuat { get; set; }
        [DisplayName("Tổng tiền")]
        public decimal? TongTien { get; set; }



        //NAVIGATION PROPERTIES
        [DisplayName("Mã đơn đặt hàng")]
        public int MaDonHang { get; set; }

        [ForeignKey("MaDonHang")]
        public DonHang DonHang { get; set; }


        public int? MaKeToan { get; set; }
        [ForeignKey("MaKeToan")]
        public NhanVien KeToan { get; set; }
    }
}
