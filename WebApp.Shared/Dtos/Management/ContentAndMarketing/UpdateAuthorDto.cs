namespace WebBanSach.Shared.Dtos.AdminClient.ContentAndMarketing
{
    // Component dùng: SuaTacGia.razor (Admin - Content & Marketing)
    public record UpdateAuthorDto(
        int AuthorId,
        string AuthorName,
        string? Biography,
        // Nếu null thì giữ ảnh cũ, nếu có giá trị thì thay ảnh mới
        string? AvatarBase64,
        string? AvatarFileName
    );
}
