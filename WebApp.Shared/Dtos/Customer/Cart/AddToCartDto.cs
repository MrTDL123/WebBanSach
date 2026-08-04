namespace WebBanSach.Shared.Dtos.CustomerDtos.Cart
{
    public record AddToCartDto(
        int ProductId,
        int Quantity = 1
    );
}
