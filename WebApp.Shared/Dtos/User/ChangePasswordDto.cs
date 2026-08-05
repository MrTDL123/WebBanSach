namespace WebApp.Shared.Dtos.User
{
    // Component dùng: ChangePassword.razor (Đổi mật khẩu tài khoản cá nhân)
    public record ChangePasswordDto(
        string CurrentPassword,
        string NewPassword,
        string ConfirmNewPassword
    );
}
