using System.ComponentModel.DataAnnotations;

namespace WebApp.Shared.Dtos.Customer.Auth
{
    /// <summary>
    /// Yêu cầu cấp mới Access Token bằng Refresh Token
    /// </summary>
    public record RefreshTokenRequestDto
    {
        [Required(ErrorMessage = "Access Token không được để trống")]
        public string AccessToken { get; init; } = string.Empty;

        [Required(ErrorMessage = "Refresh Token không được để trống")]
        public string RefreshToken { get; init; } = string.Empty;
    }
}
