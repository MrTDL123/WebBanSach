namespace WebApp.Shared.Dtos.Management.ContentAndMarketing
{
    // Component dùng: ThemSach.razor (Admin - Content & Marketing)
    public record CreateProductDto(
        string Title,
        string? Description,
        decimal Price,
        decimal DiscountPercent,
        int StockQuantity,
        string? SupplierName,
        int CategoryId,
        int AuthorId,
        int PublisherId,
        string? MainImageBase64 = null,
        string? MainImageFileName = null,
        List<string>? SecondaryImagesBase64 = null
    );
}
