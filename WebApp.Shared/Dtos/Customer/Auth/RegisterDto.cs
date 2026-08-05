namespace WebApp.Shared.Dtos.Customer.Auth
{
    // Component dùng: Register.razor (Form đăng ký tài khoản khách hàng mới)
    public record RegisterDto(
        string FullName,
        string Email,
        string PhoneNumber,
        string Password,
        string ConfirmPassword,
        string OtpCode,
        string? Address = null
    );
}
