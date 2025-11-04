using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Media.Models
{
    /// <summary>
    /// Phieu tra hang (mot lan tra hoac mot dot tra); lien ket voi DonHang (MaDonHang)
    /// </summary>
    /// 
    public enum TrangThai
    {
        [Description("Chờ duyệt")]
        ChoDuyet,
        [Description("Đã duyệt")]
        DaDuyet
    }

    public class PhieuTraHang
    {
        [Key]
        public int MaPhieuTraHang { get; set; }

        [Required]
        public int MaDonHang { get; set; }
        [Required]
        public int MaNhanVien { get; set; }

        [Required]
        public int MaKhachHang { get; set; }
        [DataType(DataType.Date)]
        [DisplayName("Ngày trả")]
        public DateTime NgayTra { get; set; } = DateTime.UtcNow;
        public TrangThai TrangThai { get; set; } = TrangThai.ChoDuyet;

        [ForeignKey(nameof(MaDonHang))]
        public DonHang DonHang { get; set; }

        [ForeignKey(nameof(MaKhachHang))]
        public KhachHang KhachHang { get; set; }
        [ForeignKey(nameof(MaNhanVien))]
        public NhanVien NhanVien { get; set; }

        public ICollection<ChiTietTraHang> ChiTietTraHangs { get; set; }
    }
}