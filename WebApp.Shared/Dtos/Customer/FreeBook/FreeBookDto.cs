namespace WebBanSach.Shared.Dtos.CustomerDtos.FreeBook
{
    // Component dùng: TuSach.razor, ChiTietTuSach.razor (Customer)
    public record FreeBookDto(
        int FreeBookId,
        string Title,
        string AuthorName,
        string? Description,
        string? CoverImageUrl,
        int AvailableQty,
        string Condition,
        string? Source,
        bool IsActive
    );
}
