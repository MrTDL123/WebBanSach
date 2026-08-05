namespace WebApp.Shared.Dtos.Management.Auth
{
    // Component dùng: AdminLogin.razor (Giao diện đăng nhập dành riêng cho Admin / Nhân viên nội bộ)
    public record AdminLoginDto(
        string UsernameOrEmail,
        string Password
    );
}
