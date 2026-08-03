using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;

namespace WebApp.Api.Entities;

public partial class User : IdentityUser
{

    public string FullName { get; set; } = null!;
    public string? AvatarUrl { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public string? Address { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;


    public virtual Cart? Cart { get; set; }
    public virtual ICollection<BookDonation> BookDonationDonors { get; set; } = new List<BookDonation>();

    public virtual ICollection<BookDonation> BookDonationReceivedByEmployees { get; set; } = new List<BookDonation>();

    public virtual ICollection<BookDonation> BookDonationReviewedByEmployees { get; set; } = new List<BookDonation>();

    public virtual ICollection<BookRequest> BookRequestCustomers { get; set; } = new List<BookRequest>();

    public virtual ICollection<BookRequest> BookRequestReviewedByEmployees { get; set; } = new List<BookRequest>();


    public virtual ICollection<CustomerAddress> CustomerAddresses { get; set; } = new List<CustomerAddress>();

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();

    public virtual ICollection<ProductReview> ProductReviewCustomers { get; set; } = new List<ProductReview>();

    public virtual ICollection<ProductReview> ProductReviewRepliedByEmployees { get; set; } = new List<ProductReview>();

    public virtual ICollection<PurchaseOrder> PurchaseOrders { get; set; } = new List<PurchaseOrder>();

    public virtual ICollection<ReturnRequest> ReturnRequestCustomers { get; set; } = new List<ReturnRequest>();

    public virtual ICollection<ReturnRequest> ReturnRequestHandledByEmployees { get; set; } = new List<ReturnRequest>();

    public virtual ICollection<Shipment> Shipments { get; set; } = new List<Shipment>();

    public virtual ICollection<SupportMessage> SupportMessages { get; set; } = new List<SupportMessage>();

    public virtual ICollection<SupportTicket> SupportTicketAssignedEmployees { get; set; } = new List<SupportTicket>();

    public virtual ICollection<SupportTicket> SupportTicketCustomers { get; set; } = new List<SupportTicket>();

    public virtual ICollection<Wishlist> Wishlists { get; set; } = new List<Wishlist>();
}
