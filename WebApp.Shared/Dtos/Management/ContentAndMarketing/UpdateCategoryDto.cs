namespace WebApp.Shared.Dtos.Management.ContentAndMarketing
{
    // Component dùng: SuaDanhMuc.razor (Admin - Content & Marketing)
    // File này trước đây hoàn toàn rỗng
    public record UpdateCategoryDto
    {
        public int CategoryId { get; set; }
        public string? CategoryName { get; set; }
        public string? Description { get; set; }
        public bool IsActive { get; set; }
        public UpdateCategoryDto() { }
    }
}
