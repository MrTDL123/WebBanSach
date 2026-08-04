namespace WebBanSach.Shared.Dtos.AdminClient.ContentAndMarketing
{
    // Component dùng: Menu danh mục, Dropdown filter (Customer), QuanLyDanhMuc.razor (Admin)
    public record CategoryDto(
        int CategoryId,
        string CategoryName,
        string? Description,
        int TotalProducts,
        bool IsActive
    );
}
