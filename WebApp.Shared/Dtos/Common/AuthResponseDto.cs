namespace WebApp.Shared.Dtos.Common
{
    /// <summary>
    /// Kết quả trả về sau khi Đăng nhập thành công (cho cả CustomerLogin và AdminLogin)
    /// </summary>
    public record AuthResponseDto(
        // Xóa Comment 2 thuộc tính nếu sau này có nhu cầu JWT Token
        //string? Token = null,
        //DateTime? Expiration = null,
        string? UserId,
        string? FullName,
        string? Email,
        string? AvatarUrl,
        List<string> Roles
    );
}
