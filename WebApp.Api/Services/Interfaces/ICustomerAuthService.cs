using WebApp.Shared.Dtos.Common;
using WebApp.Shared.Dtos.Customer.Auth;
using WebApp.Shared.Dtos.User;

namespace WebApp.Api.Services.Interfaces
{
    public interface ICustomerAuthService
    {
        Task<ApiResponse<AuthResponseDto>> LoginCustomerAsync(CustomerLoginDto dto);
        Task<ApiResponse<AuthResponseDto>> LoginCustomerJwtAsync(CustomerLoginDto dto, string? deviceInfo = null);
        Task<ApiResponse<AuthResponseDto>> RegisterCustomerAsync(RegisterDto dto, bool isMobile = false, string? deviceInfo = null);
        Task<ApiResponse<AuthResponseDto>> RefreshTokenAsync(RefreshTokenRequestDto dto, string? deviceInfo = null);
        Task<ApiResponse<bool>> RevokeTokenAsync(string refreshToken, string userId);
        Task<ApiResponse<AuthResponseDto>> GetCurrentUserAsync(string userId);
        Task<ApiResponse<AuthResponseDto>> UpdateProfileAsync(string userId, UpdateProfileDto dto);
        Task<ApiResponse<bool>> ChangePasswordAsync(string userId, ChangePasswordDto dto);
    }
}
