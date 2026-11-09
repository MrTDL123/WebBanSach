using WebApp.Shared.Dtos.Customer.Auth;

namespace WebApp.Customer.Client.Pages.Auth
{
    public partial class Register
    {
        private RegisterDto registerModel = new();
        private bool isSubmitting = false;
        private bool isSendingOtp = false;
        private bool showPassword = false;
        private bool showConfirmPassword = false;
        private string? errorMessage;
        private string? successMessage;
        private string? otpNotice;
        private List<string>? validationErrors;

        private void TogglePasswordVisibility()
        {
            showPassword = !showPassword;
        }

        private void ToggleConfirmPasswordVisibility()
        {
            showConfirmPassword = !showConfirmPassword;
        }

        private async Task HandleSendOtp()
        {
            if (string.IsNullOrWhiteSpace(registerModel.PhoneNumber) && string.IsNullOrWhiteSpace(registerModel.Email))
            {
                otpNotice = "Vui lòng nhập số điện thoại hoặc email trước khi nhận mã OTP.";
                return;
            }

            isSendingOtp = true;
            otpNotice = null;

            try
            {
                // Giả lập gửi OTP nhanh trong giai đoạn phát triển / kiểm thử
                await Task.Delay(600);
                registerModel.OtpCode = "123456";
                otpNotice = "Mã OTP thử nghiệm [123456] đã được điền tự động!";
            }
            finally
            {
                isSendingOtp = false;
            }
        }

        private async Task HandleSubmitAsync()
        {
            isSubmitting = true;
            errorMessage = null;
            successMessage = null;
            validationErrors = null;

            try
            {
                var result = await AuthService.RegisterAsync(registerModel);

                if (result.Success)
                {
                    successMessage = "Chúc mừng quý độc giả đã đăng ký tài khoản thành công! Đang chuyển hướng...";
                    StateHasChanged();
                    await Task.Delay(1500);
                    Navigation.NavigateTo("/dang-nhap");
                }
                else
                {
                    errorMessage = result.Message ?? "Đăng ký không thành công. Vui lòng kiểm tra lại thông tin.";
                    validationErrors = result.Errors;
                }
            }
            catch (Exception)
            {
                errorMessage = "Không thể kết nối đến máy chủ. Vui lòng thử lại sau.";
            }
            finally
            {
                isSubmitting = false;
            }
        }
    }
}
