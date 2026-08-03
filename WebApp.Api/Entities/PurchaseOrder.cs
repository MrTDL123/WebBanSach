using System;
using System.Collections.Generic;

namespace WebApp.Api.Entities;

public partial class PurchaseOrder
{
    public int PurchaseOrderId { get; set; }

    public int SupplierId { get; set; }

    public string CreatedByEmployeeId { get; set; } = null!;

    public DateTime ImportDate { get; set; }

    public decimal TotalCost { get; set; }

    public string? Notes { get; set; }

    public virtual User CreatedByEmployee { get; set; } = null!;

    public virtual ICollection<PurchaseOrderItem> PurchaseOrderItems { get; set; } = new List<PurchaseOrderItem>();

    public virtual Supplier Supplier { get; set; } = null!;
}
