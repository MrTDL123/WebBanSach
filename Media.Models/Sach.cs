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
        public int Id { get; set; }
        [Required]
        [DisplayName("Tên sách")]
        public string TenSach { get; set; }

        [Display(Name = "Miêu tả sản phẩm")]
        public string? MoTa { get; set; }
        [Required]
        [Display(Name = "Giá sản phẩm")]
        public double GiaBan { get; set; }
        [ValidateNever]
        public string? ImageUrl { get; set; }
        [Required]
        public DateTime? NgayCapNhap { get; set; }
        [Required]
        public int SoLuong {  get; set; }

        //NAVIGATION PROPERTIES
        public int TacGiaId { get; set; }
        public int NhaXuatBanId { get; set; }
        public int ChuDeId { get; set; }
        [ForeignKey("ChuDeId")]
        [ValidateNever]
        public ChuDe ChuDe { get; set; }

        [ForeignKey("TacGiaId")]
        [ValidateNever]
        public TacGia TacGia { get; set; }

        [ForeignKey("NhaXuatBanId")]
        [ValidateNever]
        public NhaXuatBan NhaXuatBan { get; set; }


    }
}
