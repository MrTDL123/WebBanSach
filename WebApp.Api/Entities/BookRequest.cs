using System;
using System.Collections.Generic;

namespace WebApp.Api.Entities;

public partial class BookRequest
{
    public int RequestId { get; set; }

    public string RequestCode { get; set; } = null!;

    public string CustomerId { get; set; } = null!;

    public int FreeBookId { get; set; }

    public int QuantityRequested { get; set; }

    public string Reason { get; set; } = null!;

    public string? IntendedUse { get; set; }

    public string ReceiverName { get; set; } = null!;

    public string ReceiverPhone { get; set; } = null!;

    public string ShippingAddress { get; set; } = null!;

    public int Status { get; set; }

    public string? ReviewedByEmployeeId { get; set; }

    public DateTime? ReviewedAt { get; set; }

    public string? RejectedReason { get; set; }

    public string? EmployeeNote { get; set; }

    public string? ShipmentTrackingNumber { get; set; }

    public DateTime? ShippedAt { get; set; }

    public DateTime RequestedAt { get; set; }

    public virtual User Customer { get; set; } = null!;

    public virtual FreeBook FreeBook { get; set; } = null!;

    public virtual User? ReviewedByEmployee { get; set; }
}
