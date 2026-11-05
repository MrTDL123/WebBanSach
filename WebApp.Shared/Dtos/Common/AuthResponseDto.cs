namespace WebApp.Shared.Dtos.Common
{
    /// <summary>
    /// Kết quả trả về sau khi Đăng nhập / Lấy thông tin cá nhân (hỗ trợ cả Cookie và JWT Bearer cho Web & Mobile)
    /// </summary>
    public record AuthResponseDto(
        string? UserId,
        string? FullName,
        string? Email,
        string? AvatarUrl,
        List<string> Roles,
        string? SecurityStamp = null,
        string? AccessToken = null,
        string? RefreshToken = null,
        DateTime? AccessTokenExpiration = null,
        string? PhoneNumber = null,
        DateTime? DateOfBirth = null,
        string? Address = null,
        string? Gender = null
    );
}
