using Media.Models.ViewModels;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace Media.Models
{
    public class ThanhToan
    {
        [Required(ErrorMessage = "Vui lòng nhập họ và tên")]
        [Display(Name = "Họ và tên người nhận")]
        [StringLength(100, ErrorMessage = "Họ tên không được vượt quá 100 ký tự")]
        public string HoTen { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập số điện thoại")]
        [Display(Name = "Số điện thoại")]
        [Phone(ErrorMessage = "Số điện thoại không hợp lệ")]
        [RegularExpression(@"^[0-9]{10}$", ErrorMessage = "Số điện thoại phải có 10 chữ số")]
        public string SoDienThoai { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn tỉnh/thành phố")]
        [Display(Name = "Tỉnh/Thành Phố")]
        public string TinhThanh { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn quận/huyện")]
        [Display(Name = "Quận/Huyện")]
        public string QuanHuyen { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn phường/xã")]
        [Display(Name = "Phường/Xã")]
        public string PhuongXa { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập địa chỉ nhận hàng")]
        [Display(Name = "Địa chỉ nhận hàng")]
        [StringLength(200, ErrorMessage = "Địa chỉ không được vượt quá 200 ký tự")]
        public string DiaChi { get; set; }

        [Display(Name = "Ghi chú")]
        [StringLength(50, ErrorMessage = "Ghi chú không được vượt quá 500 ký tự")]
        public string GhiChu { get; set; }

        // Order summary
        [Display(Name = "Tam tinh")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal TamTinh { get; set; }

        [Display(Name = "Phi van chuyen")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal PhiVanChuyen { get; set; }

        [Display(Name = "Tong tien")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal TongTien { get; set; }
        public string? TenSanPham { get; set; }
        public string? HinhAnhSanPham { get; set; }
        //[ValidateNever]
        public string? TenSanPham { get; set; }
        //[ValidateNever]
        public string? HinhAnhSanPham { get; set; }
        public int SoLuong { get; set; }
        public int MaDiaChiNhanHang { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn hình thức thanh toán")]
        [Display(Name = "Hình thức thanh toán")]
        public HinhThucThanhToan HinhThucThanhToanChon { get; set; } = HinhThucThanhToan.TienMatKhiNhanHang; // Đặt COD làm mặc định
        [Required(ErrorMessage = "Vui lòng chọn hình thức thanh toán")]
        [Display(Name = "Hình thức thanh toán")]
        public HinhThucThanhToan HinhThucThanhToanChon { get; set; } = HinhThucThanhToan.TienMatKhiNhanHang; // Đặt COD làm mặc định

        // Dropdown lists
        public List<SelectListItem>? DanhSachTinhThanh { get; set; }
        public List<SelectListItem>? DanhSachPhuongXa { get; set; }
        public List<SelectListItem>? DanhSachQuanHuyen { get; set; }
        public List<GioHangVM>? DanhSachSanPham { get; set; }
        public List<DiaChiNhanHang>? DanhSachDiaChi { get; set; }
        public DiaChiNhanHang? DiaChiMacDinh { get; set; }
        //[ValidateNever]
        public List<SelectListItem>? DanhSachTinhThanh { get; set; }
        //[ValidateNever]
        public List<SelectListItem>? DanhSachPhuongXa { get; set; }
        //[ValidateNever]
        public List<SelectListItem>? DanhSachQuanHuyen { get; set; }
        public List<GioHangVM>? DanhSachSanPham { get; set; }
        //[ValidateNever]
        public List<DiaChiNhanHang>? DanhSachDiaChi { get; set; }
        //[ValidateNever]
        public DiaChiNhanHang? DiaChiMacDinh { get; set; }
    }
}