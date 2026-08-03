using System;
using System.Collections.Generic;

namespace WebApp.Api.Entities;

public partial class FreeBook
{
    public int FreeBookId { get; set; }

    public string Title { get; set; } = null!;

    public string AuthorName { get; set; } = null!;

    public string? Description { get; set; }

    public string? CoverImageUrl { get; set; }

    public int AvailableQty { get; set; }

    public int TotalDonatedQty { get; set; }

    public int TotalRequestedQty { get; set; }

    public string Condition { get; set; } = null!;

    public string? Source { get; set; }

    public bool IsActive { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public virtual ICollection<BookDonationItem> BookDonationItems { get; set; } = new List<BookDonationItem>();

    public virtual ICollection<BookRequest> BookRequests { get; set; } = new List<BookRequest>();
}
