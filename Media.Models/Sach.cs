using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace Media.Models
{
    public class Sach
    {
        [Key]
        public int MaSach { get; set; }

        [Required(ErrorMessage = "Tên sách không được để trống")]
        [DisplayName("Tên sách")]
        public string TenSach { get; set; } = string.Empty;

        [Display(Name = "Mô tả sản phẩm")]
        public string? MoTa { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập giá bán")]
        [Display(Name = "Giá sản phẩm")]
        [Range(0, double.MaxValue, ErrorMessage = "Giá phải >= 0")]
        public decimal GiaBan { get; set; }

        [ValidateNever]
        public string? AnhBiaChinh { get; set; }

        [ValidateNever]
        public string? AnhBiaPhu1 { get; set; }

        [ValidateNever]
        public string? AnhBiaPhu2 { get; set; }

        [ValidateNever]
        public string? AnhBiaPhu3 { get; set; }

        [ValidateNever]
        public string? AnhBiaPhu4 { get; set; }

        [Required]
        [DisplayName("Ngày cập nhật")]
        public DateTime NgayCapNhat { get; set; } = DateTime.Now;

        [Required]
        [Range(0, int.MaxValue, ErrorMessage = "Số lượng phải >= 0")]
        public int SoLuong { get; set; }

        [Required, MaxLength(100)]
        [DisplayName("Nhà cung cấp")]
        public string NhaCungCap { get; set; } = string.Empty;

        // 🔹 Navigation Properties
        [Required]
        [DisplayName("Mã tác giả")]
        public int MaTacGia { get; set; }

        [Required]
        [DisplayName("Mã nhà xuất bản")]
        public int MaNhaXuatBan { get; set; }

        [Required]
        [DisplayName("Mã chủ đề")]
        public int MaChuDe { get; set; }

        [ForeignKey(nameof(MaChuDe))]
        [ValidateNever]
        public ChuDe? ChuDe { get; set; }

        [ForeignKey(nameof(MaTacGia))]
        [ValidateNever]
        public TacGia? TacGia { get; set; }

        [ForeignKey(nameof(MaNhaXuatBan))]
        [ValidateNever]
        public NhaXuatBan? NhaXuatBan { get; set; }

        // 🔹 Khởi tạo collection để tránh null
        public ICollection<ChiTietDonHang> ChiTietDonHangs { get; set; } = new List<ChiTietDonHang>();
        public ICollection<ChiTietTraHang> ChiTietTraHangs { get; set; } = new List<ChiTietTraHang>();
        public ICollection<ChiTietGioHang> ChiTietGioHangs { get; set; } = new List<ChiTietGioHang>();
    }
}
