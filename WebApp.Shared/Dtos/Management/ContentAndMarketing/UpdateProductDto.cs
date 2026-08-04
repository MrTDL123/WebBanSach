namespace WebBanSach.Shared.Dtos.Management.ContentAndMarketing
{
    // Component dùng: SuaSach.razor (Admin - Content & Marketing)
    public record UpdateProductDto(
        int ProductId,
        string Title,
        string? Description,
        decimal Price,
        decimal DiscountPercent,
        int StockQuantity,
        string? SupplierName,
        int CategoryId,
        int AuthorId,
        int PublisherId,
        bool IsActive,
        string? MainImageBase64 = null,
        string? MainImageFileName = null
    );
}
