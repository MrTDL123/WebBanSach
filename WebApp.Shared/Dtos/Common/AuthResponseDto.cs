namespace WebBanSach.Shared.Dtos.Common
{
    // Kết quả trả về sau khi Đăng nhập thành công (cho cả CustomerLogin và AdminLogin)
    public record AuthResponseDto(
        bool Success,
        string Message,
        string? Token = null,
        DateTime? Expiration = null,
        string? UserId = null,
        string? FullName = null,
        string? Email = null,
        string? AvatarUrl = null,
        List<string>? Roles = null
    );
}
