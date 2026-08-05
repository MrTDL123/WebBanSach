namespace WebApp.Shared.Dtos.AdminClient.SystemAdmin
{
    // Component dùng: ThemNhanVien.razor (Admin)
    public record CreateEmployeeDto(
        string FullName,
        string Email,
        string PhoneNumber,
        string Password,
        string Role,
        string? Address = null,
        DateTime? DateOfBirth = null
    );
}
