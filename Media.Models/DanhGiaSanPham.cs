using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
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
    public class DanhGiaSanPham
    {
        [Key]
        public int MaDanhGia { get; set; }

        [Required]
        public int MaSach { get; set; }
        [ForeignKey("MaSach")]
        [ValidateNever]
        public Sach Sach { get; set; }

        [Required]
        public int MaKhachHang { get; set; }
        [ForeignKey("MaKhachHang")]
        [ValidateNever]
        public KhachHang KhachHang { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn số sao")]
        [Range(1, 5)]
        public int SoSao { get; set; } = 5; // Mặc định 5 sao

        [MaxLength(2000)]
        [Display(Name = "Nhận xét")]
        public string NoiDung { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập tên hiển thị")]
        [MaxLength(100)]
        [Display(Name = "Tên hiển thị")]
        public string TenHienThi { get; set; }

        [Display(Name = "Đánh giá ẩn danh")]
        public bool IsAnDanh { get; set; } = false;

        public DateTime NgayDang { get; set; } = DateTime.UtcNow;
        [Display(Name = "Lượt thích")]
        [DefaultValue(0)]
        public int LuotThich { get; set; } = 0;
        [ValidateNever]
        public ICollection<LuotThichDanhGiaSanPham> LuotThichDanhGiaSanPhams { get; set; }
    }
}