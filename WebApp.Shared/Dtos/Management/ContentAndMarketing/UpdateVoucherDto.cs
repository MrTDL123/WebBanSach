using System;
using System.Collections.Generic;
using System.Text;

namespace WebApp.Shared.Dtos.Management.ContentAndMarketing
{
    public record UpdateVoucherDto
    {
        public int VoucherId { get; set; }
        public string? Code { get; set; }
        public string? Description { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal DiscountPercent { get; set; }
        public decimal MinOrderAmount { get; set; }
        public decimal MaxDiscountAmount { get; set; }
        public int TotalUsageLimit { get; set; }
        public DateTime ExpiredAt { get; set; }
        public bool IsActive { get; set; }

        public UpdateVoucherDto() { }
    }
}
