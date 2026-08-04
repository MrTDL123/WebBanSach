namespace WebBanSach.Shared.Dtos.Customer.Auth
{
    // Component dùng: ResetPassword.razor (Đặt lại mật khẩu mới bằng mã OTP)
    public record ResetPasswordDto(
        string Email,
        string OtpCode,
        string NewPassword,
        string ConfirmNewPassword
    );
}
