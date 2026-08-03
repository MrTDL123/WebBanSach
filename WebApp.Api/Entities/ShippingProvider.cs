using System;
using System.Collections.Generic;

namespace WebApp.Api.Entities;

public partial class ShippingProvider
{
    public int ProviderId { get; set; }

    public string ProviderName { get; set; } = null!;

    public string? ContactPhone { get; set; }

    public bool IsActive { get; set; }

    public virtual ICollection<BookDonation> BookDonations { get; set; } = new List<BookDonation>();

    public virtual ICollection<Shipment> Shipments { get; set; } = new List<Shipment>();
}
