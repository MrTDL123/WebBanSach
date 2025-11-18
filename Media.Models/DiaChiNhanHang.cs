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
    public class DiaChiNhanHang
    {
        [Key]
        public int MaDiaChi { get; set; }

        [Required]
        public int MaKhachHang { get; set; }

        [Required, StringLength(50)]
        [DisplayName("Tỉnh/Thành phố")]
        public string TinhThanh { get; set; }

        [Required, StringLength(50)]
        [DisplayName("Quận/Huyện")]
        public string QuanHuyen { get; set; }

        [Required, StringLength(50)]
        [DisplayName("Phường/Xã")]
        public string PhuongXa { get; set; }
        public int? MaTinhThanh { get; set; }
        public int? MaQuanHuyen { get; set; }
        public int? MaPhuongXa { get; set; }

        [Required, StringLength(200)]
        [DisplayName("Địa chỉ chi tiết")]
        public string DiaChiChiTiet { get; set; }
        [StringLength(100)]
        [DisplayName("Tên người nhận hàng")]
        public string TenNguoiNhan { get; set; }

        [StringLength(20)]
        [DisplayName("Số điện thoại nhận hàng")]
        public string SoDienThoai { get; set; }

        [DisplayName("Là địa chỉ mặc định?")]
        public bool LaMacDinh { get; set; } = false;
        [DefaultValue(false)]
        public bool LaDaXoa { get; set; } = false;

        [ForeignKey(nameof(MaKhachHang))]
        [ValidateNever]
        public KhachHang KhachHang { get; set; }
        [ValidateNever]
        public ICollection<DonHang> DonHangs { get; set; }
    }
}