using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Media.Models.ViewModels
{
    public class ThemNhanVienVM
    {
        public NhanVien NhanVienMoi { get; set; }
        [Required(ErrorMessage = "Mật khẩu không được bỏ trống")]
        [DataType(DataType.Password)]
        public string MatKhau { get; set; }

    }
}
