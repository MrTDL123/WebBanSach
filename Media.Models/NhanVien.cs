using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
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
        public int MaNhanVien { get; set; }

        [Required]
        [ValidateNever] // THÊM DÒNG NÀY - QUAN TRỌNG!
        public string MaTaiKhoan { get; set; }

        [Required]
        [StringLength(150)]
        [DisplayName("Họ tên")]
        public string HoTen { get; set; }

        [Required, StringLength(10)]
        [RegularExpression(@"^[0-9]{10}$", ErrorMessage = "So dien thoai phai co 10 chu so")]
        [DisplayName("Số diện thoại")]
        public string DienThoai { get; set; }
        [EmailAddress, StringLength(200)]
        [DisplayName("Email")]
        public string Email { get; set; }

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
        [Column(TypeName = "decimal(18,2)")]
        public decimal? Luong { get; set; }

        [DisplayName("Bậc lương")]
        public int? BacLuong { get; set; }

        [DisplayName("Ngày vào làm")]
        [DataType(DataType.Date)]
        public DateTime? NgayVaoLam { get; set; }

        [StringLength(200)]
        [DisplayName("Quê quán")]
        public string QueQuan { get; set; }

        // NAVIGATION PROPERTIES
        [ValidateNever]
        [ForeignKey("MaTaiKhoan")]
        public TaiKhoan TaiKhoan { get; set; }

        [ValidateNever]
        public ICollection<DonHang> DonHangs { get; set; }

        [ValidateNever]
        public ICollection<PhieuTraHang> PhieuTraHangs { get; set; }

        [ValidateNever]
        public ICollection<ChamSocKhachHang> ChamSocKhachHangs { get; set; }
    }
}