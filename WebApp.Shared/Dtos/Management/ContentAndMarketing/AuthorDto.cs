namespace WebBanSach.Shared.Dtos.AdminClient.ContentAndMarketing
{
    // Component dùng: TrangTacGia.razor (Customer), Dropdown chọn tác giả (Admin)
    public record AuthorDto(
        int AuthorId,
        string AuthorName,
        string? Biography,
        string? AvatarUrl,
        int TotalBooks
    );
}
