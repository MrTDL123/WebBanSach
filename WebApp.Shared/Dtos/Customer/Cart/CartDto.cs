namespace WebApp.Shared.Dtos.CustomerDtos.Cart
{
    public record CartDto(
        List<CartItemDto> Items
    )
    {
        public decimal TotalAmount => Items?.Sum(i => i.SubTotal) ?? 0;
        public int TotalItems => Items?.Sum(i => i.Quantity) ?? 0;
    }
}
