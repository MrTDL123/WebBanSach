using WebApp.Admin.Services.Base;
using WebApp.Admin.Services.Interfaces;
using WebApp.Admin.Utilities;
using WebApp.Shared.Dtos.Common;
using WebApp.Shared.Dtos.User;

namespace WebApp.Admin.Services.Implementations
{
    public class UserClientService : BaseApiClient, IUserClientService
    {
        public UserClientService(
            HttpClient httpClient,
            UserSessionState userSessionState,
            IHttpContextAccessor httpContextAccessor)
            : base(httpClient, userSessionState, httpContextAccessor)
        {
        }
        /// <summary>
        /// Xử lý lấy, sửa thông tin người dùng và mật khẩu trang thông tin cá nhân
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
    }
}
