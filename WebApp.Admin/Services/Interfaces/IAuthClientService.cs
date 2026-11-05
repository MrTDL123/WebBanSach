using WebApp.Shared.Dtos.Common;
using WebApp.Shared.Dtos.Management.Auth;
using WebApp.Shared.Dtos.User;

namespace WebApp.Admin.Services.Interfaces
{
    public interface IAuthClientService
    {
        Task<ApiResponse<AuthResponseDto>> LoginAsync(AdminLoginDto dto);
        Task<ApiResponse<AuthResponseDto>> GetCurrentUserAsync();
        //Task<ApiResponse<bool>> LogoutAsync();
        Task<ApiResponse<bool>> ForgotPasswordAsync(AdminForgotPasswordDto dto);
        Task<ApiResponse<bool>> VerifyOtpAsync(AdminVerifyOtpDto dto);
        Task<ApiResponse<bool>> ResetPasswordAsync(AdminResetPasswordDto dto);
        // Kiểm tra Security Stamp để xác thực người dùng có còn trong trạng thái đăng nhập hay không
        Task<ApiResponse<ValidateStampDto>> ValidateSecurityStampAsync();
    }
}
