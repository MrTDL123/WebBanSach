using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Forms;
using WebApp.Shared.Dtos.Management.Auth;

namespace WebApp.Admin.Components.Pages.Auth
{
    [AllowAnonymous]
    public partial class ForgotPassword
    {
        private AdminResetPasswordDto _forgotPasswordModel = new();
        private EditContext? _editContext;
        private int _currentStep = 1;
        private bool _isOtpSent = false;
        private bool _isTimerRunning = false;
        private bool _isSubmitting = false;
        private bool _showNewPassword = false;
        private bool _showConfirmPassword = false;
        private int _countdownSeconds = 60;
        private string? _errorMessage;

        private List<string>? _validationErrors;

        protected override void OnInitialized()
        {
            _editContext = new EditContext(_forgotPasswordModel);
        }

        // Điều kiện Validate cho các nút
        private bool CanSendOtp => !string.IsNullOrWhiteSpace(_forgotPasswordModel.Email) && !_isTimerRunning && !_isOtpSent;
        private bool CanVerifyOtp => _isOtpSent && _forgotPasswordModel.OtpCode?.Trim().Length == 6;
        private bool CanResetPassword => !string.IsNullOrWhiteSpace(_forgotPasswordModel.NewPassword)
                                       && _forgotPasswordModel.NewPassword == _forgotPasswordModel.ConfirmNewPassword
                                       && _forgotPasswordModel.NewPassword.Length >= 6;

        private void ToggleShowNewPassword() => _showNewPassword = !_showNewPassword;
        private void ToggleShowConfirmPassword() => _showConfirmPassword = !_showConfirmPassword;

        private async Task HandleSendOtp()
        {
            if (_isSubmitting || _isTimerRunning) return;
            var fieldIdentifier = new FieldIdentifier(_forgotPasswordModel, nameof(_forgotPasswordModel.Email));
            _editContext?.NotifyFieldChanged(fieldIdentifier);

            if (_editContext?.GetValidationMessages(fieldIdentifier).Any() == true)
            {
                _errorMessage = "Vui lòng nhập email hợp lệ.";
                return;
            }

            _isSubmitting = true;
            _errorMessage = null;

            try
            {
                // Đóng gói DTO (AdminForgotPasswordDto tự có Constructor ngầm từ C# record, không cần viết tay như Model form)
                var result = await AuthClientService.ForgotPasswordAsync(new AdminForgotPasswordDto(_forgotPasswordModel.Email));
                if (result.Success)
                {
                    _isOtpSent = true;
                    // Chạy đếm ngược ngầm (Fire-and-forget), không dùng await để tránh treo UI suốt 60s
                    // Dấu gạch "_" gọi là Discard là cách bạn nói với c# "Tôi cố tình cho nó chạy ngầm dưới nền, tôi biết mình đang làm gì, đừng hiện cảnh báo vàng nữa!".
                    _ = StartTimer();
                }
                else
                {
                    _errorMessage = result.Message ?? "Gửi OTP thất bại.";
                }
            }
            catch (Exception)
            {
                _errorMessage = "Không thể kết nối đến máy chủ. Vui lòng thử lại sau.";
            }
            finally
            {
                _isSubmitting = false;
            }
        }

        private async Task HandleVerifyOtp()
        {
            // 1. Tạo định danh cho riêng ô Email
            var fieldIdentifier = new FieldIdentifier(_forgotPasswordModel, nameof(_forgotPasswordModel.OtpCode));
            // 2. Ép _editContext/FluentValidator chạy validate ĐÚNG ô Email này
            _editContext?.NotifyFieldChanged(fieldIdentifier);
            // 3. Kiểm tra xem ô Email có bị dính lỗi nào không
            if (_editContext?.GetValidationMessages(fieldIdentifier).Any() == true)
            {
                _errorMessage = "Vui lòng nhập mã OTP 6 số hợp lệ.";
                return;
            }

            _isSubmitting = true;
            _errorMessage = null;

            try
            {
                var result = await AuthClientService.VerifyOtpAsync(new AdminVerifyOtpDto(_forgotPasswordModel.Email, _forgotPasswordModel.OtpCode));
                if (result.Success)
                {
                    _currentStep = 2; // Chuyển bước thành công
                }
                else
                {
                    _errorMessage = result.Message ?? "Mã OTP không chính xác hoặc đã hết hạn.";
                }
            }
            catch (Exception)
            {
                _errorMessage = "Lỗi kết nối máy chủ khi xác thực OTP.";
            }
            finally
            {
                _isSubmitting = false;
            }
        }

        private async Task HandleResetPassword()
        {
            if (_editContext?.Validate() == false)
            {
                _errorMessage = "Vui lòng điền đầy đủ thông tin hợp lệ.";
                return;
            }

            _isSubmitting = true;
            _errorMessage = null;
            _validationErrors = null;

            // TODO: Gọi API Reset Password (AdminResetPasswordDto)
            try
            {
                var result = await AuthClientService.ResetPasswordAsync(_forgotPasswordModel);
                if (result.Success)
                {
                    Navigation.NavigateTo("/management/login");
                }
                else
                {
                    // Hứng thông báo lỗi chung và danh sách lỗi chi tiết từ API
                    _errorMessage = result.Message ?? "Đặt lại mật khẩu thất bại.";
                    _validationErrors = result.Errors;
                }
            }
            catch (Exception)
            {
                _errorMessage = "Không thể kết nối đến máy chủ. Vui lòng kiểm tra lại kết nối mạng.";
            }
            finally
            {
                _isSubmitting = false; // Trả lại trạng thái cho UI
            }
        }

        private async Task StartTimer()
        {
            _isTimerRunning = true;
            _countdownSeconds = 60;
            while (_countdownSeconds > 0)
            {
                await Task.Delay(1000);
                _countdownSeconds--;
                StateHasChanged();
            }
            _isTimerRunning = false;
        }
    }
}
