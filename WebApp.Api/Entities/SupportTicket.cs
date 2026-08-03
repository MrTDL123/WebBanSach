using System;
using System.Collections.Generic;

namespace WebApp.Api.Entities;

public partial class SupportTicket
{
    public int TicketId { get; set; }

    public string CustomerId { get; set; } = null!;

    public string Subject { get; set; } = null!;

    public int Status { get; set; }

    public string? AssignedEmployeeId { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public virtual User? AssignedEmployee { get; set; }

    public virtual User Customer { get; set; } = null!;

    public virtual ICollection<SupportMessage> SupportMessages { get; set; } = new List<SupportMessage>();
}
