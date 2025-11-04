using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Media.Models
{
    public class ChiTietGioHang
    {
        // Khóa chính tổng hợp (MaGioHang + MaSach)
        [Key, Column(Order = 0)]
        public int MaGioHang { get; set; }

        [Key, Column(Order = 1)]
        public int MaSach { get; set; }

        [Required(ErrorMessage = "Số lượng không được để trống")]
        [Range(1, int.MaxValue, ErrorMessage = "Số lượng phải lớn hơn 0")]
        [DisplayName("Số lượng")]
        public int SoLuong { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        [DisplayName("Đơn giá")]
        public decimal DonGia { get; set; }

        [NotMapped]
        public decimal ThanhTien => SoLuong * DonGia;

        [ForeignKey(nameof(MaGioHang))]
        public GioHang GioHang { get; set; }

        [ForeignKey(nameof(MaSach))]
        public Sach Sach { get; set; }
    }
}