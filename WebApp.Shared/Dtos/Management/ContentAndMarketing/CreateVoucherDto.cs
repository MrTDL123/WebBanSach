using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebApp.Shared.Dtos.AdminClient.ContentAndMarketing
{
    // Admin tạo Voucher mới
    public record CreateVoucherDto(
        string Code,
        string? Description,
        decimal DiscountAmount,
        decimal DiscountPercent,
        decimal MinOrderAmount,
        decimal MaxDiscountAmount,
        int TotalUsageLimit,
        DateTime ExpiredAt
    );
}
