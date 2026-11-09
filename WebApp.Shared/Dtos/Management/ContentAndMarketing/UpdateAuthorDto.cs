namespace WebApp.Shared.Dtos.Management.ContentAndMarketing
{
    // Component dùng: SuaTacGia.razor (Admin - Content & Marketing)
    public record UpdateAuthorDto
    {
        public int AuthorId { get; set; }
        public string? AuthorName { get; set; }
        public string? Biography { get; set; }
        public bool IsActive { get; set; }

        public UpdateAuthorDto() { }
    }  
}
