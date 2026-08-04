namespace WebBanSach.Shared.Dtos.CustomerDtos.Order
{
    public record OrderItemDto(
        int ProductId,
        string Title,
        string? ImageUrl,
        int Quantity,
        decimal UnitPrice
    )
    {
        public decimal SubTotal => UnitPrice * Quantity;
    }
}
