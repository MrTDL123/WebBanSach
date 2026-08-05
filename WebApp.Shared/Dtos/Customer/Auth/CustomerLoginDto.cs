namespace WebApp.Shared.Dtos.Customer.Auth
{
    // Component dùng: CustomerLogin.razor (Form đăng nhập khách hàng trên Web Storefront)
    public record CustomerLoginDto(
        string EmailOrPhone,
        string Password
    );
}
