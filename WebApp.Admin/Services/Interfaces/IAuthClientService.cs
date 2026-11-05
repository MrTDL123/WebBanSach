using WebApp.Shared.Dtos.Common;
using WebApp.Shared.Dtos.Management.Auth;
using WebApp.Shared.Dtos.User;

namespace WebApp.Admin.Services.Interfaces
{
    public interface IAuthClientService
    {
        /// <summary>
        /// Đăng nhập cho Admin
        /// </summary>
        Task<ApiResponse<AuthResponseDto>> LoginAsync(AdminLoginDto dto);

        /// <summary>
        /// Lấy thông tin người dùng hiện tại (Admin) dựa trên cookie
        /// </summary>
        Task<ApiResponse<AuthResponseDto>> GetCurrentUserAsync();

        /// <summary>
        /// Quên mật khẩu, xác minh OTP, đặt lại mật khẩu cho Admin
        /// </summary>
        Task<ApiResponse<bool>> ForgotPasswordAsync(AdminForgotPasswordDto dto);
        Task<ApiResponse<bool>> VerifyOtpAsync(AdminVerifyOtpDto dto);
        Task<ApiResponse<bool>> ResetPasswordAsync(AdminResetPasswordDto dto);
    }
}
