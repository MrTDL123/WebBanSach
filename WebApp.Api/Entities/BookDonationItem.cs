using System;
using System.Collections.Generic;

namespace WebApp.Api.Entities;

public partial class BookDonationItem
{
    public int ItemId { get; set; }

    public int DonationId { get; set; }

    public string BookTitle { get; set; } = null!;

    public string? AuthorName { get; set; }

    public int Quantity { get; set; }

    public string? Condition { get; set; }

    public string? Note { get; set; }

    public int? ActualQuantityReceived { get; set; }

    public int? LinkedFreeBookId { get; set; }

    public virtual BookDonation Donation { get; set; } = null!;

    public virtual FreeBook? LinkedFreeBook { get; set; }
}
