using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Media.Models
{
    public class NhanVien
    {
        [Key]
        [DisplayName("Mã nhân viên")]
        public int MaNhanVien { get; set; }

        [Required]
        [StringLength(150)]
        [DisplayName("Họ tên")]
        public string HoTen { get; set; }

        [StringLength(300)]
        [DisplayName("Địa chỉ")]
        public string DiaChi { get; set; }

        [DataType(DataType.Date)]
        [DisplayName("Ngày sinh")]
        public DateTime? NgaySinh { get; set; }

        [StringLength(30)]
        [DisplayName("CCCD")]
        public string CCCD { get; set; }

        [DisplayName("Lương")]
        public decimal? Luong { get; set; }

        [DisplayName("Bậc lương")]
        public int? BacLuong { get; set; }

        [DisplayName("Ngày vào làm")]
        public DateTime? NgayVaoLam { get; set; }

        [StringLength(200)]
        [DisplayName("Quê quán")]
        public string QueQuan { get; set; }


        //NAVIGATION PROPERTIES
        public string MaTaiKhoan { get; set; }
        [ForeignKey("MaTaiKhoan")]
        public TaiKhoan TaiKhoan { get; set; }

        public ICollection<DonHang> DonHangs { get; set; }
        public ICollection<PhieuNhapKho> PhieuNhapKhos { get; set; }
        public ICollection<KiemKeSanPham> KiemKeSanPhams { get; set; }
        public ICollection<ChamSocKhachHang> ChamSocKhachHangs { get; set; }
    }
}
