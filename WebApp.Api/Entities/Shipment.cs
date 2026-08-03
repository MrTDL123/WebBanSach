using System;
using System.Collections.Generic;

namespace WebApp.Api.Entities;

public partial class Shipment
{
    public int ShipmentId { get; set; }

    public int OrderId { get; set; }

    public int ProviderId { get; set; }

    public string TrackingNumber { get; set; } = null!;

    public string ShippingStatus { get; set; } = null!;

    public DateTime? EstimatedDeliveryDate { get; set; }

    public DateTime? ActualDeliveryDate { get; set; }

    public string? ProcessedByEmployeeId { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual Order Order { get; set; } = null!;

    public virtual User? ProcessedByEmployee { get; set; }

    public virtual ShippingProvider Provider { get; set; } = null!;
}
