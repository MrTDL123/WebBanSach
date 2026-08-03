using System;
using System.Collections.Generic;

namespace WebApp.Api.Entities;

public partial class Coupon
{
    public int CouponId { get; set; }

    public string Code { get; set; } = null!;

    public decimal DiscountAmount { get; set; }

    public decimal MinOrderAmount { get; set; }

    public DateTime StartDate { get; set; }

    public DateTime EndDate { get; set; }

    public int UsageLimit { get; set; }

    public int TimesUsed { get; set; }

    public bool IsActive { get; set; }
}
