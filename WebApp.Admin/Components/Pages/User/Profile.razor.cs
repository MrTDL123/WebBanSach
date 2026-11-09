using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Forms;
using System.ComponentModel;
using System.Security.Claims;
using WebApp.Admin.Auth;
using WebApp.Shared.Dtos.Common;
using WebApp.Shared.Dtos.User;
using WebApp.Admin.Utilities;

namespace WebApp.Admin.Components.Pages.User
{
    public partial class Profile
    {
        [CascadingParameter]
        private Task<AuthenticationState>? AuthStateTask { get; set; }
        // DTO chứa toàn bộ thông tin Read-only từ DB để hiển thị UI
        private UserProfileDto? _userProfileModel;
        // Model cho tab Thông tin cá nhân
        private UpdateProfileDto _updateProfileModel = new();

        // Model cho tab Đổi mật khẩu
        private ChangePasswordDto _passwordModel = new();

        // Trạng thái UI
        private bool _isLoading = true;
        private bool _isSubmittingProfile = false;
        private bool _isSubmittingPassword = false;
        private string? _errorMessage;
        private string? _successMessage;
        private string _activeTab { get; set; } = "info";
        private bool _showCurrentPwd { get; set; } = false;
        private bool _showNewPwd { get; set; } = false;
        private bool _showConfirmPwd { get; set; } = false;
        private IBrowserFile? _selectedAvatarFile;
        private string? _avatarPreviewUrl;

        private void ChangeTab(string tabName) => _activeTab = tabName;
        private void ToggleCurrentPwd() => _showCurrentPwd = !_showCurrentPwd;
        private void ToggleNewPwd() => _showNewPwd = !_showNewPwd;
        private void ToggleConfirmPwd() => _showConfirmPwd = !_showConfirmPwd;

        protected override async Task OnInitializedAsync()
        {
            await LoadUserProfileAsync();
        }

        private async Task LoadUserProfileAsync()
        {
            _isLoading = true;
            _errorMessage = null;

            try
            {
                // Gọi Service lấy thông tin User từ DB qua API Backend
                var response = await UserClientService.GetUserProfileAsync();

                if (response.Success && response.Data != null)
                {
                    var userProfile = response.Data;

                    // Map dữ liệu từ API vào Model hiển thị trên form
                    _updateProfileModel = new UpdateProfileDto
                    {
                        FullName = userProfile.FullName,
                        DateOfBirth = userProfile.DateOfBirth,
                        Address = userProfile.Address
                    };

                    _userProfileModel = userProfile;
                }
                else
                {
                    _errorMessage = response?.Message ?? "Không thể tải thông tin hồ sơ.";
                }
            }
            catch (Exception ex)
            {
                _errorMessage = $"Đã xảy ra lỗi khi tải dữ liệu: {ex.Message}";
            }
            finally
            {
                _isLoading = false;
            }
        }

        // 1. Hàm chạy khi người dùng chọn file ảnh từ máy tính
        private async Task HandleAvatarSelected(InputFileChangeEventArgs e)
        {
            if (e.File != null)
            {
                try
                {
                    _selectedAvatarFile = e.File;
                    _avatarPreviewUrl = await ImageHelper.ConvertToBase64Async(e.File);
                }
                catch (Exception ex)
                {
                    _errorMessage = "Lỗi xử lý ảnh: " + ex.Message;
                }
            }
        }

        // Xử lý Cập nhật Hồ sơ Cá nhân
        private async Task HandleUpdateProfileAsync()
        {
            _isSubmittingProfile = true;
            _errorMessage = null;
            _successMessage = null;

            // 🔥 ĐÓNG GÓI CHUỖI BASE64 VÀO MODEL ĐỂ GỬI SANG API
            if (!string.IsNullOrEmpty(_avatarPreviewUrl) && _selectedAvatarFile != null)
            {
                _updateProfileModel.AvatarBase64 = _avatarPreviewUrl;
                _updateProfileModel.AvatarFileName = _selectedAvatarFile.Name;
            }

            var result = await UserClientService.UpdateUserProfileAsync(_updateProfileModel);

            if (result.Success)
            {
                _successMessage = "Cập nhật thông tin hồ sơ thành công!";
                await LoadUserProfileAsync();
                _avatarPreviewUrl = null;
                _selectedAvatarFile = null;

                // 🔥 THÔNG BÁO CHO TOPHEADER CẬP NHẬT LẠI AVATAR MỚI TỨC THÌ
                if (AuthStateProvider is CustomAuthStateProvider customAuth)
                {
                    await customAuth.RefreshAuthenticationStateAsync();
                }
            }
            else
            {
                _errorMessage = result?.Message ?? "Cập nhật hồ sơ thất bại.";
            }

            _isSubmittingProfile = false;
        }

        // 3. Hàm tính toán đường dẫn hiển thị ảnh Avatar chuẩn xác
        private string GetAvatarDisplayUrl()
        {
            if (!string.IsNullOrEmpty(_avatarPreviewUrl))
            {
                return _avatarPreviewUrl;
            }
            if (!string.IsNullOrEmpty(_userProfileModel?.AvatarUrl))
            {
                return _userProfileModel.AvatarUrl.StartsWith("http")
                    ? _userProfileModel.AvatarUrl
                    : $"https://localhost:7188{_userProfileModel.AvatarUrl}";
            }
            return $"https://ui-avatars.com/api/?name={Uri.EscapeDataString(_userProfileModel?.FullName ?? "Admin")}&background=E31837&color=fff&size=128";
        }

        // Xử lý Đổi Mật Khẩu
        private async Task HandleChangePasswordAsync()
        {
            _isSubmittingPassword = true;
            _errorMessage = null;
            _successMessage = null;

            var result = await UserClientService.ChangePasswordAsync(_passwordModel);

            if (result.Success)
            {
                _successMessage = "Đổi mật khẩu thành công!";
                _passwordModel = new ChangePasswordDto(); // Reset form mật khẩu
            }
            else
            {
                _errorMessage = result?.Message ?? "Đổi mật khẩu thất bại.";
            }

            _isSubmittingPassword = false;
        }

        private void OpenChangeEmailModal()
        {

        }

        private void OpenChangePhoneModal()
        {

        }
    }
}
