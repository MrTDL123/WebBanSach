namespace WebApp.Shared.Dtos.Management.ContentAndMarketing
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
