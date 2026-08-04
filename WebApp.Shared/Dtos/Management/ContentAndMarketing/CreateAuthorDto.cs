namespace WebBanSach.Shared.Dtos.AdminClient.ContentAndMarketing
{
    // Component dùng: ThemTacGia.razor (Admin - Content & Marketing)
    public record CreateAuthorDto(
        string AuthorName,
        string? Biography,
        string? AvatarBase64,
        string? AvatarFileName
    );
}
