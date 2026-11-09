using Microsoft.AspNetCore.Components;
using WebApp.Shared.Dtos.Management.Admin;

namespace WebApp.Admin.Components.Pages.Admin
{
    public partial class BanUserModal
    {
        private bool _isOpen = false;
        private bool _isSubmitting = false;
        private string? _errorMessage;
        private string _userName = string.Empty;

        private BanUserDto _banModel = new();

        [Parameter]
        public EventCallback OnSuccess { get; set; }

        public void OpenForBan(string userId, string userName, bool isStaff)
        {
            _userName = userName;
            _banModel = new BanUserDto
            {
                UserId = userId,
                Reason = string.Empty
            };
            _errorMessage = null;
            _isOpen = true;
            StateHasChanged();
        }

        public void Close()
        {
            _isOpen = false;
            _errorMessage = null;
        }

        private async Task HandleSubmitAsync()
        {
            _isSubmitting = true;
            _errorMessage = null;

            try
            {
                var result = await AdminClientService.BanUserAsync(_banModel);
                if (!result.Success)
                {
                    _errorMessage = result.Message ?? "Khóa tài khoản thất bại";
                    Toast.ShowError(_errorMessage);
                    return;
                }

                Close();
                Toast.ShowSuccess($"Đã khóa tài khoản {_userName} thành công");
                await OnSuccess.InvokeAsync();
            }
            catch (Exception ex)
            {
                _errorMessage = $"Đã xảy ra lỗi: {ex.Message}";
            }
            finally
            {
                _isSubmitting = false;
            }
        }
    }
}