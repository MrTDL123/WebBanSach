namespace WebApp.Shared.Dtos.Management.ContentAndMarketing
{
    // Component dùng: ThemTacGia.razor (Admin - Content & Marketing)
    public record CreateAuthorDto
    {
        public string? AuthorName { get; set; }
        public string? Biography { get; set; }

        public CreateAuthorDto() { }
        public CreateAuthorDto(string? authorName, string? biography)
        {
            AuthorName = authorName;
            Biography = biography;
        }
    }
}
