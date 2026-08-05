namespace WebApp.Shared.Dtos.AdminClient.ContentAndMarketing
{
    // Component dùng: SuaDanhMuc.razor (Admin - Content & Marketing)
    // File này trước đây hoàn toàn rỗng
    public record UpdateCategoryDto(
        int CategoryId,
        string CategoryName,
        string? Description,
        bool IsActive
    );
}
