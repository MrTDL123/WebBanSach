namespace WebApp.Shared.Dtos.Management.Admin
{
    // Component dùng: QuanLyNguoiDung.razor (Admin)
    public record UserListItemDto(
        string UserId,
        string FullName,
        string? Email,
        string? PhoneNumber,
        bool IsActive,
        DateTime CreatedAt,
        List<string> Roles,
        int TotalOrders
    );
}
