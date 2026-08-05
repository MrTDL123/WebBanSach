namespace WebApp.Shared.Dtos.AdminClient.ContentAndMarketing
{
    // Component dùng: ThemNXB.razor (Admin - Content & Marketing)
    public record CreatePublisherDto(
        string PublisherName,
        string? Address,
        string? PhoneNumber,
        string? Email,
        string? Website
    );
}
