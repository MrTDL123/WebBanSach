namespace WebApp.Shared.Dtos.CustomerDtos.Cart
{
    public record CartItemDto(
        int ProductId,
        string Title,
        string? MainImageUrl,
        decimal UnitPrice,
        decimal FinalUnitPrice,
        int Quantity,
        int StockQuantity
    )
    {
        public decimal SubTotal => FinalUnitPrice * Quantity;
    }
}
