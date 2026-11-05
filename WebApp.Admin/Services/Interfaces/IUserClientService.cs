using WebApp.Shared.Dtos.Common;
using WebApp.Shared.Dtos.User;

namespace WebApp.Admin.Services.Interfaces
{
    public interface IUserClientService
    {
        /// <summary>
        /// Lấy thông tin hồ sơ người dùng hiện tại (Admin)
        /// </summary>
        Task<ApiResponse<UserProfileDto>> GetUserProfileAsync();

        /// <summary>
        /// Cập nhật thông tin hồ sơ người dùng hiện tại và thay đổi mật khẩu (Admin)
        /// </summary>
        Task<ApiResponse<bool>> UpdateUserProfileAsync(UpdateProfileDto dto);
        Task<ApiResponse<bool>> ChangePasswordAsync(ChangePasswordDto dto);
    }
}
