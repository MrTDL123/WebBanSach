namespace WebBanSach.Shared.Dtos.AdminClient.SystemAdmin
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
