using System;
using System.Collections.Generic;

namespace WebApp.Api.Entities;

public partial class ProductReview
{
    public int ReviewId { get; set; }

    public int ProductId { get; set; }

    public string CustomerId { get; set; } = null!;

    public int Rating { get; set; }

    public string? Comment { get; set; }

    public string? ReplyFromShop { get; set; }

    public string? RepliedByEmployeeId { get; set; }

    public DateTime? RepliedAt { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual User Customer { get; set; } = null!;

    public virtual Product Product { get; set; } = null!;

    public virtual User? RepliedByEmployee { get; set; }
}
