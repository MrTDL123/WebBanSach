using System.ComponentModel.DataAnnotations;

namespace WebApp.Shared.Dtos.Customer.Auth
{
    /// <summary>
    /// Yêu cầu thu hồi (revoke) Refresh Token khi đăng xuất hoặc hủy phiên
    /// </summary>
    public record RevokeTokenRequestDto
    {
        [Required(ErrorMessage = "Refresh Token không được để trống")]
        public string RefreshToken { get; init; } = string.Empty;
    }
}
