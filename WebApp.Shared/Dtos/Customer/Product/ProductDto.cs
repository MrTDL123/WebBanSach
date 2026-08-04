namespace WebBanSach.Shared.Dtos.CustomerDtos.Product
{
    // Component dùng: ProductDetail.razor (Customer)
    public record ProductDto(
        int ProductId,
        string Title,
        string? Description,
        decimal Price,
        decimal DiscountPercent,
        int StockQuantity,
        string? MainImageUrl,
        List<string> SecondaryImageUrls,
        bool IsActive,
        int CategoryId,
        string? CategoryName,
        int AuthorId,
        string? AuthorName,
        int PublisherId,
        string? PublisherName,
        string? SupplierName,
        double AverageRating,
        int TotalReviews
    )
    {
        public decimal FinalPrice => Price * (1 - DiscountPercent / 100m);
    }
}
