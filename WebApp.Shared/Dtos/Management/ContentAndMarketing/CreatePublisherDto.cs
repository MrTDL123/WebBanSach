namespace WebApp.Shared.Dtos.Management.ContentAndMarketing
{
    // Component dùng: ThemNXB.razor (Admin - Content & Marketing)
    public record CreatePublisherDto
    {
        public string? PublisherName { get; set; }
        public string? Address { get; set; }
        public string? PhoneNumber { get; set; }

        public CreatePublisherDto() { }
        public CreatePublisherDto(string? publisherName, string? address, string? phoneNumber)
        {
            PublisherName = publisherName;
            Address = address;
            PhoneNumber = phoneNumber;
        }
    }
}
