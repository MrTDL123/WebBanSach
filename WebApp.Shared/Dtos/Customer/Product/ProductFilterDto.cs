namespace WebBanSach.Shared.Dtos.CustomerDtos.Product
{
    // Component dùng: ProductList.razor, SearchResults.razor (Customer)
    public record ProductFilterDto(
        string? Keyword = null,
        int? CategoryId = null,
        int? AuthorId = null,
        int? PublisherId = null,
        decimal? MinPrice = null,
        decimal? MaxPrice = null,
        string SortBy = "newest",
        int Page = 1,
        int PageSize = 20
    );
}
