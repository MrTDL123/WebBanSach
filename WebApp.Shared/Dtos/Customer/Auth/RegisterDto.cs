namespace WebApp.Shared.Dtos.Customer.Auth
{
    // Component dùng: Register.razor (Form đăng ký tài khoản khách hàng mới trên Web và Mobile)
    public record RegisterDto
    {
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string ConfirmPassword { get; set; } = string.Empty;
        public string? OtpCode { get; set; } = string.Empty;
        public string? Address { get; set; }

        public RegisterDto() { }

        public RegisterDto(
            string fullName,
            string email,
            string phoneNumber,
            string password,
            string confirmPassword,
            string? otpCode = null,
            string? address = null)
        {
            FullName = fullName;
            Email = email;
            PhoneNumber = phoneNumber;
            Password = password;
            ConfirmPassword = confirmPassword;
            OtpCode = otpCode;
            Address = address;
        }
    }
}
