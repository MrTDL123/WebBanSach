namespace WebApp.Shared.Dtos.Management.ContentAndMarketing
{
    // Component dùng: SuaNXB.razor (Admin - Content & Marketing)
    // File này trước đây hoàn toàn rỗng
    public record UpdatePublisherDto
    {
        public int PublisherId { get; set; }
        public string? PublisherName { get; set; }
        public string? Address { get; set; }
        public string? PhoneNumber { get; set; }
        public bool IsActive { get; set; }

        public UpdatePublisherDto() { }
    }
}
