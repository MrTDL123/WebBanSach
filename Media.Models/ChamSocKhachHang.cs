using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Media.Models
{
    public class ChamSocKhachHang
    {
        [Key]
        public int MaChamSoc { get; set; }
        [Required]
        public int MaNhanVien { get; set; }
        [Required]
        public int MaKhachHang { get; set; }

        [DisplayName("Nội dung")]
        public string NoiDung { get; set; }

        [DataType(DataType.Date)]
        [DisplayName("Ngày chăm sóc")]
        public DateTime? NgayChamSoc { get; set; } = DateTime.Now;
        [ForeignKey(nameof(MaNhanVien))]
        public NhanVien NhanVien { get; set; }
        [ForeignKey(nameof(MaKhachHang))]
        public KhachHang KhachHang { get; set; }
    }
}