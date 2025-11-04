using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Media.Models
{
    /// <summary>
    /// Chi tiet cua phieu tra hang (mot dong cho mot sach)
    /// </summary>
    public class ChiTietTraHang
    {
        [Key, Column(Order = 0)]
        public int MaPhieuTraHang { get; set; }

        [Key, Column(Order = 1)]
        public int MaSach { get; set; }

        [Range(1, int.MaxValue)]
        [Required, DisplayName("Số lượng trả")]
        public int SoLuongTra { get; set; } = 1;
        [DisplayName("Đơn giá hoàn")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal DonGiaHoan { get; set; }
        [DisplayName("Tổng tiền hoàn")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal TongTienHoan { get; set; }

        [Required, StringLength(255)]
        [DisplayName("Lý do trả hàng")]
        public string LyDoTraHang { get; set; }

        [ForeignKey(nameof(MaPhieuTraHang))]
        public PhieuTraHang PhieuTraHang { get; set; }

        [ForeignKey(nameof(MaSach))]
        public Sach Sach { get; set; }
    }
}