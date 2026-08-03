using System;
using System.Collections.Generic;

namespace WebApp.Api.Entities;

public partial class PurchaseOrderItem
{
    public int ItemNo { get; set; }

    public int PurchaseOrderId { get; set; }

    public int ProductId { get; set; }

    public int QuantityImported { get; set; }

    public decimal ImportUnitPrice { get; set; }

    public virtual Product Product { get; set; } = null!;

    public virtual PurchaseOrder PurchaseOrder { get; set; } = null!;
}
