namespace WebApp.Shared.Dtos.Management.ContentAndMarketing
{
    // Component dùng: ThemDanhMuc.razor (Admin - Content & Marketing)
    public record CreateCategoryDto
    {
        public string? CategoryName { get; set; }
        public string? Description { get; set; }

        public CreateCategoryDto() { }
        public CreateCategoryDto(string? categoryName, string? description)
        {
            CategoryName = categoryName;
            Description = description;
        }
    }
}
