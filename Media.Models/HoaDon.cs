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
        [DisplayName("Mã đơn đặt hàng")]
        public int MaDonHang { get; set; }

        [DataType(DataType.Date)]
        [DisplayName("Ngày lập")]
        public DateTime? NgayLap { get; set; }
        [DisplayName("VAT")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal VAT { get; set; }
        [DisplayName("Tổng tiền")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal? TongTien { get; set; }

        //NAVIGATION PROPERTIES
        
        [ForeignKey("MaDonHang")]
        public DonHang DonHang { get; set; }
    }
}