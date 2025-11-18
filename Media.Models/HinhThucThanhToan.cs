using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Media.Models
{
    public enum HinhThucThanhToan
    {
        [Description("Thanh toán tiền mặt khi nhận hàng")]
        TienMatKhiNhanHang,
        [Description("Chuyển khoản")]
        ChuyenKhoan
    }
}