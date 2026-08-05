namespace WebApp.Shared.Dtos.CustomerDtos.Order
{
    // Component dùng: ChiTietDonHang.razor (Customer & Admin)
    public record OrderDto(
        int OrderId,
        string OrderCode,
        DateTime OrderDate,
        int Status,
        string StatusText,
        int PaymentMethod,
        string PaymentMethodText,
        bool IsPaid,
        string ReceiverName,
        string ReceiverPhone,
        string ShippingAddress,
        decimal SubTotal,
        decimal ShippingFee,
        decimal DiscountAmount,
        decimal TotalAmount,
        string? Notes,
        string? TrackingNumber,
        string? ShippingProviderName,
        List<OrderItemDto> Items
    );
}
