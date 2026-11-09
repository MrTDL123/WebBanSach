using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebApp.Shared.Dtos.Management.ContentAndMarketing
{
    // Admin tạo Voucher mới
    public record CreateVoucherDto
    {
        public string? Code { get; set; }
        public string? Description { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal DiscountPercent { get; set; }
        public decimal MinOrderAmount { get; set; }
        public decimal MaxDiscountAmount { get; set; }
        public int TotalUsageLimit { get; set; }
        public DateTime ExpiredAt { get; set; }

        public CreateVoucherDto() { }
    }
}
