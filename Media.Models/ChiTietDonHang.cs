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
    public class ChiTietDonHang
    {
        [Key, Column(Order = 0)]
        public int MaDonHang { get; set; }
        [Key, Column(Order = 1)]
        public int MaSach { get; set; }
        [Required]
        [DisplayName("Số lượng"), Range(1, int.MaxValue)]
        public int SoLuong { get; set; }
        [Required]
        [DisplayName("Đơn giá")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal DonGia { get; set; }
        [Required]
        [DisplayName("Thành tiền")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal ThanhTien { get; set; }
        [ForeignKey(nameof(MaDonHang))]
        public DonHang DonHang { get; set; }
        [ForeignKey(nameof(MaSach))]
        public Sach Sach { get; set; }
    }
}