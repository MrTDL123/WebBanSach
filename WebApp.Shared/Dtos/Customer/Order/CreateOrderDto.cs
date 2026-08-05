namespace WebApp.Shared.Dtos.CustomerDtos.Order
{
    // Component dùng: Checkout.razor (Customer)
    public record CreateOrderDto(
        string ReceiverName,
        string ReceiverPhone,
        string Province,
        string District,
        string Ward,
        string DetailAddress,
        string? Notes = null,
        int PaymentMethod = 0,
        string? CouponCode = null,
        int? SavedAddressId = null
    );
}
