namespace WebApp.Shared.Dtos.Customer.Auth
{
    // Component dùng: CustomerLogin.razor (Form đăng nhập khách hàng trên Web Storefront)
    public record CustomerLoginDto
    {
        public string EmailOrPhone { get; set; } = string.Empty;
        public string Password {  get; set; } = string.Empty;
        public CustomerLoginDto() { }

        public CustomerLoginDto(string emailOrPhone, string password)
        {
            EmailOrPhone = emailOrPhone;
            Password = password;
        }
    }
}
