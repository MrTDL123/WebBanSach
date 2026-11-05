namespace WebApp.Shared.Dtos.Management.Admin
{
    // Component dùng: ThemNhanVien.razor (Admin)
    public record CreateEmployeeDto
    {
        public string? FullName { get; set; }
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Password { get; set; }
        public string? Role { get; set; }
        public string? Address { get; set; }
        public DateTime? DateOfBirth { get; set; }

        public CreateEmployeeDto() { }
    }
}
