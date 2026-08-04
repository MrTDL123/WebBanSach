namespace WebBanSach.Shared.Dtos.CustomerDtos.Product
{
    // Component dùng: ProductCard.razor, Home.razor (Customer), QuanLySach.razor (Admin)
    public record ProductListItemDto(
        int ProductId,
        string Title,
        string? AuthorName,
        string? CategoryName,
        decimal Price,
        decimal DiscountPercent,
        string? MainImageUrl,
        int StockQuantity,
        bool IsActive,
        double AverageRating
    )
    {
        public decimal FinalPrice => Price * (1 - DiscountPercent / 100m);
    }
}
