using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Media.Models
{
    /// <summary>
    /// Gio hang cua khach (one-to-one voi KhachHang)
    /// </summary>
    public class GioHang
    {
        [Key]
        public int MaKhachHang { get; set; }
        [DataType(DataType.Date)]
        [DisplayName("Ngày tạo")]
        public DateTime NgayTao { get; set; } = DateTime.UtcNow;
        [Required]
        [DisplayName("Tổng tiền")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal TongTien { get; set; }

        [ForeignKey(nameof(MaKhachHang))]
        public KhachHang KhachHang { get; set; }
        public ICollection<ChiTietGioHang> ChiTietGioHangs { get; set; }
    }
}