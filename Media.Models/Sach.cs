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
        [StringLength(255, ErrorMessage = "Tên sách không được vượt quá 255 ký tự")]
        public string TenSach { get; set; } = string.Empty;

        [Display(Name = "Mô tả sản phẩm")]
        public string? MoTa { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập giá bán")]
        [Display(Name = "Giá sản phẩm")]
        [Column(TypeName = "decimal(18,2)")]
        [Range(0, double.MaxValue, ErrorMessage = "Giá bán phải lớn hơn 0")]
        public decimal GiaBan { get; set; }

        // SỬA: Phần trăm giảm giá nên là decimal với độ chính xác phù hợp
        [Display(Name = "Phần trăm giảm giá")]
        [Column(TypeName = "decimal(5,2)")]
        [Range(0, 100, ErrorMessage = "Phần trăm giảm giá phải từ 0 đến 100")]
        public decimal PhanTramGiamGia { get; set; } = 0;

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

        [Required(ErrorMessage = "Nhà cung cấp không được để trống")]
        [MaxLength(100, ErrorMessage = "Nhà cung cấp không được vượt quá 100 ký tự")]
        [DisplayName("Nhà cung cấp")]
        public string NhaCungCap { get; set; } = string.Empty;

        // 🔹 Navigation Properties
        [Required(ErrorMessage = "Vui lòng chọn tác giả")]
        [DisplayName("Mã tác giả")]
        public int MaTacGia { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn nhà xuất bản")]
        [DisplayName("Mã nhà xuất bản")]
        public int MaNhaXuatBan { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn chủ đề")]
        [DisplayName("Mã chủ đề")]
        public int MaChuDe { get; set; }

        // SỬA: Tính toán đúng giá sau giảm
        [NotMapped]
        public decimal GiaSauGiam => GiaBan * (1 - PhanTramGiamGia);

        [ForeignKey(nameof(MaChuDe))]
        [ValidateNever]
        public ChuDe? ChuDe { get; set; }

        [ForeignKey(nameof(MaTacGia))]
        [ValidateNever]
        public TacGia? TacGia { get; set; }

        [ForeignKey(nameof(MaNhaXuatBan))]
        [ValidateNever]
        public NhaXuatBan? NhaXuatBan { get; set; }

        public ICollection<ChiTietDonHang>? ChiTietDonHangs { get; set; }
        public ICollection<ChiTietTraHang>? ChiTietTraHangs { get; set; }
        public ICollection<ChiTietGioHang>? ChiTietGioHangs { get; set; }
        public ICollection<DanhGiaSanPham>? DanhGiaSanPhams { get; set; }
    }
}