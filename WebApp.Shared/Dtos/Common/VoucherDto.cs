namespace WebApp.Shared.Dtos.Common
{
    // Component dùng: ApDungMaGiamGia.razor (Customer - Checkout)
    public record VoucherDto(
        int VoucherId,
        string Code,
        string? Description,
        decimal DiscountAmount,
        decimal DiscountPercent,
        decimal MinOrderAmount,
        decimal MaxDiscountAmount,
        int RemainingUsage,
        DateTime ExpiredAt,
        bool IsActive
    );
}
