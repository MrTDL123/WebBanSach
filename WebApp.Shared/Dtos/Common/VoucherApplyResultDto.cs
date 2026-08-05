namespace WebApp.Shared.Dtos.Common
{
    // Component dùng: ApDungMaGiamGia.razor (Customer - Checkout result)
    public record VoucherApplyResultDto(
        bool IsValid,
        string? ErrorMessage,
        decimal DiscountAmount,
        decimal FinalAmount
    );
}
