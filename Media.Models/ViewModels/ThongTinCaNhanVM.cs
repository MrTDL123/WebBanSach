using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Media.Models.ViewModels
{
    public class ThongTinCaNhanVM
    {
        public KhachHang ThongTinKhachHang { get; set; }
        public int SoLuongDonHang { get; set; }
        [Display(Name = "Ngày")]
        public int NgaySinh { get; set; }

        [Display(Name = "Tháng")]
        public int ThangSinh { get; set; }

        [Display(Name = "Năm")]
        public int NamSinh { get; set; }
        public GioiTinh? gender { get; set; }
        [Required]
        public string MaOTP { get; set; }

        // Dùng để chứa danh sách cho dropdown
        [ValidateNever]
        public IEnumerable<SelectListItem> CacNgay { get; set; }
        [ValidateNever]
        public IEnumerable<SelectListItem> CacThang { get; set; }
        [ValidateNever]
        public IEnumerable<SelectListItem> CacNam { get; set; }
    }
}
