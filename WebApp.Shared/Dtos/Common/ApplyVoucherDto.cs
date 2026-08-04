using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebBanSach.Shared.Dtos.Common
{
    // Khách nhập mã voucher ở trang Checkout
    public record ApplyVoucherDto(
        string Code,
        decimal CurrentOrderAmount    // Để API kiểm tra điều kiện MinOrderAmount
    );
}
