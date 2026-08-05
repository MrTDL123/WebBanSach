namespace WebApp.Shared.Dtos.CustomerDtos.Order
{
    // Component dùng: LichSuDonHang.razor (Customer), QuanLyDonHang.razor (Admin)
    public record OrderListItemDto(
        int OrderId,
        string OrderCode,
        DateTime OrderDate,
        int Status,
        string StatusText,
        decimal TotalAmount,
        bool IsPaid,
        string FirstProductTitle,
        string? FirstProductImageUrl,
        int FirstProductQuantity,
        int OtherProductsCount,
        string? CustomerName = null,
        string? CustomerPhone = null
    );
}
