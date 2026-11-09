using WebApp.Admin.Services.Base;
using WebApp.Admin.Services.Interfaces;
using WebApp.Admin.Utilities;
using WebApp.Shared.Dtos.Common;
using WebApp.Shared.Dtos.Management.Auth;
using WebApp.Shared.Dtos.User;

namespace WebApp.Admin.Services.Implementations
{
    public class AuthClientService : BaseApiClient, IAuthClientService
    {
        public AuthClientService(
            HttpClient httpClient, 
            UserSessionState userSessionState,
            IHttpContextAccessor httpContextAccessor)
            : base(httpClient, userSessionState, httpContextAccessor)
        {
        }

        /// <summary>
        /// Xử lý đăng nhập, đăng xuất cho management và lấy thông tin người dùng hiện tại
        /// </summary>
        public Task<ApiResponse<AuthResponseDto>> LoginAsync(AdminLoginDto loginDto)
        { 
            return PostAsJsonAsync<AdminLoginDto, AuthResponseDto>("api/auth/admin-login", loginDto); 
        }

        public Task<ApiResponse<AuthResponseDto>> GetCurrentUserAsync()
        {
            return ExecuteApiAsync<AuthResponseDto>(() => _httpClient.GetAsync("api/auth/admin-me"));
        }

        /// <summary>
        /// Xử lý Forgot Password, Verify OTP và Reset Password cho management
        /// </summary>
        public Task<ApiResponse<bool>> ForgotPasswordAsync(AdminForgotPasswordDto dto) 
        { 
            return PostAsJsonAsync<AdminForgotPasswordDto, bool>("api/auth/admin-forgot-password", dto);
        }

        public Task<ApiResponse<bool>> VerifyOtpAsync(AdminVerifyOtpDto dto) 
        { 
            return PostAsJsonAsync<AdminVerifyOtpDto, bool>("api/auth/admin-verify-otp", dto);
        }

        public Task<ApiResponse<bool>> ResetPasswordAsync(AdminResetPasswordDto dto) 
        { 
            return PostAsJsonAsync<AdminResetPasswordDto, bool>("api/auth/admin-reset-password", dto);
        }

        /// <summary>
        /// Xử lý lấy thông tin hồ sơ người dùng, cập nhật hồ sơ và đổi mật khẩu cho management
        /// </summary>

        public Task<ApiResponse<UserProfileDto>> GetUserProfileAsync()
        {
            return GetAsync<UserProfileDto>("api/user/profile");
        }

        public Task<ApiResponse<bool>> UpdateUserProfileAsync(UpdateProfileDto dto)
        {
            return PutAsJsonAsync<UpdateProfileDto, bool>("api/user/profile", dto);
        }

        public Task<ApiResponse<bool>> ChangePasswordAsync(ChangePasswordDto dto)
        {
            return PostAsJsonAsync<ChangePasswordDto, bool>("api/user/change-password", dto);
        }

        public Task<ApiResponse<ValidateStampDto>> ValidateSecurityStampAsync()
        {
            return GetAsync<ValidateStampDto>("api/auth/validate-stamps");
        }
    }
}
