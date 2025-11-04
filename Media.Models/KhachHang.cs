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
    public class KhachHang
    {
        [Key]
        public int MaKhachHang { get; set; }
        [Required]
        [StringLength(50)]
        [DisplayName("Họ và tên")]
        public string HoTen { get; set; }
        [Required, StringLength(10)]
        [RegularExpression(@"^[0-9]{10}$", ErrorMessage = "So dien thoai phai co 10 chu so")]
        [DisplayName("Số diện thoại")]
        public string DienThoai { get; set; }
        [EmailAddress, StringLength(200)]
        [DisplayName("Email")]
        public string Email { get; set; }
        [DisplayName("Địa chỉ")]
        public string? DiaChi { get; set; }
        [DisplayName("Ngày sinh")]
        [DataType(DataType.Date)]
        public DateTime? NgaySinh { get; set; }


        //NAVIGATION PROPERTIES
        public string MaTaiKhoan { get; set; }
        [ForeignKey("MaTaiKhoan")]
        public TaiKhoan TaiKhoan { get; set; }
        public GioHang GioHang { get; set; }

        public ICollection<DonHang> DonHangs { get; set; }
        public ICollection<PhanHoiKhachHang> PhanHoiKhachHangs { get; set; }
        public ICollection<ChamSocKhachHang> ChamSocKhachHangs { get; set; }
        public ICollection<DiaChiNhanHang> DiaChiNhanHangs { get; set; }
    }
}