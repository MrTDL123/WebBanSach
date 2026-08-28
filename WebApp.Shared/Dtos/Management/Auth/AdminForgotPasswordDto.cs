namespace WebApp.Shared.Dtos.Management.Auth
{
    // Component dùng: ForgotPassword.razor (Yêu cầu gửi mã OTP khôi phục mật khẩu)
    // Cú pháp ngắn gọn: C# tự sinh Constructor có tham số ngầm (Chỉ dùng làm DTO gửi API, không binding Form)
    public record AdminForgotPasswordDto(
        string Email
    );
}
