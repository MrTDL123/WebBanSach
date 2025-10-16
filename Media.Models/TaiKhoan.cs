
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Media.Models
{
    //Class này chỉ phục vụ thao tác đăng nhập, đăng ký
    public class TaiKhoan : IdentityUser //Vì IdentityUser thiếu các cột cần thiết nên ta sẽ implement nó qua KhachHang
    {
        public KhachHang KhachHang { get; set; } 
    }
}
