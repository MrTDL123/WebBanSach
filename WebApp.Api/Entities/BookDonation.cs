using System;
using System.Collections.Generic;

namespace WebApp.Api.Entities;

public partial class BookDonation
{
    public int DonationId { get; set; }

    public string DonationCode { get; set; } = null!;

    public string? DonorId { get; set; }

    public string DonorName { get; set; } = null!;

    public string DonorPhone { get; set; } = null!;

    public string? DonorEmail { get; set; }

    public string PickupAddress { get; set; } = null!;

    public string? DonorMessage { get; set; }

    public int Status { get; set; }

    public string? ReviewedByEmployeeId { get; set; }

    public DateTime? ReviewedAt { get; set; }

    public string? RejectedReason { get; set; }

    public int? ProviderId { get; set; }

    public string? PickupTrackingNumber { get; set; }

    public DateTime? PickupScheduledDate { get; set; }

    public DateTime? ActualPickupDate { get; set; }

    public DateTime? WarehouseReceivedAt { get; set; }

    public string? WarehouseNote { get; set; }

    public string? ReceivedByEmployeeId { get; set; }

    public DateTime DonatedAt { get; set; }

    public virtual ICollection<BookDonationItem> BookDonationItems { get; set; } = new List<BookDonationItem>();

    public virtual User? Donor { get; set; }

    public virtual ShippingProvider? Provider { get; set; }

    public virtual User? ReceivedByEmployee { get; set; }

    public virtual User? ReviewedByEmployee { get; set; }
}
