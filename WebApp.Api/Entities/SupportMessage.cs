using System;
using System.Collections.Generic;

namespace WebApp.Api.Entities;

public partial class SupportMessage
{
    public int MessageId { get; set; }

    public int TicketId { get; set; }

    public string SenderId { get; set; } = null!;

    public string MessageText { get; set; } = null!;

    public DateTime SentAt { get; set; }

    public virtual User Sender { get; set; } = null!;

    public virtual SupportTicket Ticket { get; set; } = null!;
}
