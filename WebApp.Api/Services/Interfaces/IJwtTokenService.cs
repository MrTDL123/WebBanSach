using System.Security.Claims;
using WebApp.Api.Entities;

namespace WebApp.Api.Services.Interfaces
{
    /// <summary>
    /// Service xử lý phát hành và kiểm tra JWT Access Token / Refresh Token
    /// </summary>
    public interface IJwtTokenService
    {
        /// <summary>
        /// Sinh chuỗi Access Token JWT với các claims chuẩn OAuth 2.0 và Identity
        /// </summary>
        (string Token, string JwtId, DateTime ExpiresAt) GenerateAccessToken(User user, IList<string> roles, string clientId = "android-app");

        /// <summary>
        /// Sinh chuỗi Refresh Token ngẫu nhiên an toàn (64 bytes)
        /// </summary>
        string GenerateRefreshToken();

        /// <summary>
        /// Trích xuất ClaimsPrincipal từ Access Token đã hết hạn (dùng cho quy trình Refresh Token)
        /// </summary>
        ClaimsPrincipal? GetPrincipalFromExpiredToken(string token);
    }
}
