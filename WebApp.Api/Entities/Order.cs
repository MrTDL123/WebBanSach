using System;
using System.Collections.Generic;

namespace WebApp.Api.Entities;

public partial class Order
{
    public int OrderId { get; set; }

    public string OrderCode { get; set; } = null!;

    public string UserId { get; set; } = null!;

    public DateTime OrderDate { get; set; }

    public int Status { get; set; }

    public int PaymentMethod { get; set; }

    public bool IsPaid { get; set; }

    public string ReceiverName { get; set; } = null!;

    public string ReceiverPhone { get; set; } = null!;

    public string ShippingAddress { get; set; } = null!;

    public decimal SubTotal { get; set; }

    public decimal ShippingFee { get; set; }

    public decimal DiscountAmount { get; set; }

    public decimal TotalAmount { get; set; }

    public string? Notes { get; set; }

    public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();

    public virtual ReturnRequest? ReturnRequest { get; set; }

    public virtual Shipment? Shipment { get; set; }

    public virtual User User { get; set; } = null!;
}
