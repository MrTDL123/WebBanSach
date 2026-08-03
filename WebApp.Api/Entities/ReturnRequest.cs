using System;
using System.Collections.Generic;

namespace WebApp.Api.Entities;

public partial class ReturnRequest
{
    public int ReturnRequestId { get; set; }

    public int OrderId { get; set; }

    public string CustomerId { get; set; } = null!;

    public string Reason { get; set; } = null!;

    public string? ProofImageUrl { get; set; }

    public int Status { get; set; }

    public string? BankName { get; set; }

    public string? BankAccountNumber { get; set; }

    public string? BankAccountHolder { get; set; }

    public decimal RefundAmount { get; set; }

    public string? HandledByEmployeeId { get; set; }

    public string? EmployeeNotes { get; set; }

    public DateTime RequestedAt { get; set; }

    public DateTime? ProcessedAt { get; set; }

    public virtual User Customer { get; set; } = null!;

    public virtual User? HandledByEmployee { get; set; }

    public virtual Order Order { get; set; } = null!;
}
