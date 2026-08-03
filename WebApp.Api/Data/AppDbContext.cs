using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using WebApp.Api.Entities;

namespace WebApp.Api.Data;

public partial class AppDbContext : IdentityDbContext<User, Role, string>
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public override DbSet<User> Users { get; set; }
    public override DbSet<Role> Roles { get; set; }
    public DbSet<Author> Authors { get; set; }

    public DbSet<Banner> Banners { get; set; }

    public DbSet<BookDonation> BookDonations { get; set; }

    public DbSet<BookDonationItem> BookDonationItems { get; set; }

    public DbSet<BookRequest> BookRequests { get; set; }

    public DbSet<Cart> Carts { get; set; }

    public DbSet<CartItem> CartItems { get; set; }

    public DbSet<Category> Categories { get; set; }

    public DbSet<Coupon> Coupons { get; set; }

    public DbSet<CustomerAddress> CustomerAddresses { get; set; }

    public DbSet<FreeBook> FreeBooks { get; set; }

    public DbSet<Order> Orders { get; set; }

    public DbSet<OrderItem> OrderItems { get; set; }

    public DbSet<Product> Products { get; set; }

    public DbSet<ProductImage> ProductImages { get; set; }

    public DbSet<ProductReview> ProductReviews { get; set; }

    public DbSet<Publisher> Publishers { get; set; }

    public DbSet<PurchaseOrder> PurchaseOrders { get; set; }

    public DbSet<PurchaseOrderItem> PurchaseOrderItems { get; set; }

    public DbSet<ReturnRequest> ReturnRequests { get; set; }
    
    public DbSet<Shipment> Shipments { get; set; }

    public DbSet<ShippingProvider> ShippingProviders { get; set; }

    public DbSet<Supplier> Suppliers { get; set; }

    public DbSet<SupportMessage> SupportMessages { get; set; }

    public DbSet<SupportTicket> SupportTickets { get; set; }

    public DbSet<Wishlist> Wishlists { get; set; }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("Users");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.ToTable("Roles"); 
        });

        modelBuilder.Entity<IdentityUserRole<string>>(entity =>
        {
            entity.ToTable("UserRoles");
        });

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }
}
