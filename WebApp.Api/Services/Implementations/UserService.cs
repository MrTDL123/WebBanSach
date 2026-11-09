using Microsoft.AspNetCore.Identity;
using WebApp.Api.Entities;
using WebApp.Api.Services.Interfaces;
using WebApp.Shared.Dtos.Common;
using WebApp.Shared.Dtos.User;

namespace WebApp.Api.Services.Implementations
{
    public class UserService : IUserService
    {
        private readonly UserManager<User> _userManager;
        private readonly IFileStorageService _fileStorageService;

        public UserService(UserManager<User> userManager, IFileStorageService fileStorageService)
        {
            _userManager = userManager;
            _fileStorageService = fileStorageService;
        }

        public async Task<ApiResponse<UserProfileDto>> GetUserProfileAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null || !user.IsActive)
            {
                return ApiResponse<UserProfileDto>.FailureResult("Người dùng không tồn tại hoặc đã bị khóa.");
            }

            // Map thông tin Entity sang UserProfileDto
            var profileDto = new UserProfileDto(
                UserId: user.Id,
                FullName: user.FullName,
                Email: user.Email,
                PhoneNumber: user.PhoneNumber,
                DateOfBirth: user.DateOfBirth,
                Address: user.Address,
                AvatarUrl: user.AvatarUrl,
                TotalOrders: 0,        // Phía Admin tạm để 0
                TotalWishlistItems: 0  // Phía Admin tạm để 0
            );

            return ApiResponse<UserProfileDto>.SuccessResult(profileDto);
        }

        public async Task<ApiResponse<bool>> UpdateUserProfileAsync(string userId, UpdateProfileDto dto)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null || !user.IsActive)
            {
                return ApiResponse<bool>.FailureResult("Tài khoản không tồn tại hoặc đã bị khóa.");
            }

            try
            {
                // 1. Cập nhật các thông tin cơ bản
                user.UpdateProfile(dto.FullName, dto.Address, dto.DateOfBirth);

                if (!string.IsNullOrEmpty(dto.AvatarBase64))
                {
                    // 🔥 BƯỚC DỌN RÁC TỰ ĐỘNG: Xóa ảnh cũ của User này khỏi đĩa cứng (nếu đã từng có ảnh trước đó)
                    _fileStorageService.DeletePhysicalFile(user.AvatarUrl);
                    user.UpdateAvatar(await _fileStorageService.SaveBase64ImageAsync(
                        dto.AvatarBase64, 
                        "uploads/avatars/management", 
                        "avatar"));
                }
                
                var result = await _userManager.UpdateAsync(user);
                if (!result.Succeeded)
                {
                    var errors = result.Errors.Select(e => e.Description).ToList();
                    return ApiResponse<bool>.FailureResult("Không thể cập nhật hồ sơ.", errors);
                }

                return ApiResponse<bool>.SuccessResult(true, "Cập nhật hồ sơ cá nhân thành công.");
            }
            catch (Exception ex)
            {
                return ApiResponse<bool>.FailureResult($"Đã xảy ra lỗi khi cập nhật hồ sơ: {ex.Message}");
            }
        }

        public async Task<ApiResponse<bool>> ChangePasswordAsync(string userId, ChangePasswordDto dto)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null || !user.IsActive)
            {
                return ApiResponse<bool>.FailureResult("Tài khoản không tồn tại hoặc đã bị khóa.");
            }

            // Sử dụng tính năng ChangePasswordAsync tích hợp sẵn của Identity
            var result = await _userManager.ChangePasswordAsync(user, dto.CurrentPassword, dto.NewPassword);

            if (!result.Succeeded)
            {
                var errors = result.Errors.Select(e => e.Description).ToList();
                return ApiResponse<bool>.FailureResult("Đổi mật khẩu thất bại.", errors);
            }

            return ApiResponse<bool>.SuccessResult(true, "Đổi mật khẩu thành công.");
        }
    }
}
