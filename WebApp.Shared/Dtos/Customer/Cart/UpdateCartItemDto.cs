namespace WebApp.Shared.Dtos.CustomerDtos.Cart
{
    public record UpdateCartItemDto(
        int ProductId,
        int Quantity
    );
}
