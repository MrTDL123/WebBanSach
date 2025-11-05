using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace Media.Models
{
    public class Sach
    {
        [Key]
        public int MaSach { get; set; }
        [Required]
        [DisplayName("Tên sách")]
        public string TenSach { get; set; }

        [Display(Name = "Mô tả sản phẩm")]
        public string? MoTa { get; set; }
        [Required]
        [Display(Name = "Giá sản phẩm")]
        public decimal GiaBan { get; set; }
        [ValidateNever]
        public string? ImageUrl { get; set; }
        [Required]
        [DisplayName("Ngày cập nhật")]
        public DateTime? NgayCapNhat { get; set; }
        [Required]
        public int SoLuong {  get; set; }
        [Required, MaxLength(100)]
        public string NhaCungCap { get; set; }

        //NAVIGATION PROPERTIES
        public int MaTacGia { get; set; }
        public int MaNhaXuatBan { get; set; }
        public int MaChuDe { get; set; }

        [ForeignKey("MaChuDe")]
        [ValidateNever]
        public ChuDe ChuDe { get; set; }

        [ForeignKey("MaTacGia")]
        [ValidateNever]
        public TacGia TacGia { get; set; }

        [ForeignKey("MaNhaXuatBan")]
        [ValidateNever]
        public NhaXuatBan NhaXuatBan { get; set; }

        public ICollection<ChiTietDonHang> ChiTietDonHangs { get; set; }
        public ICollection<ChiTietTraHang> ChiTietTraHangs { get; set; }
        public ICollection<ChiTietGioHang> ChiTietGioHangs { get; set; }
    }
}
