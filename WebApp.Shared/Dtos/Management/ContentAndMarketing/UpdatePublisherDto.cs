namespace WebApp.Shared.Dtos.AdminClient.ContentAndMarketing
{
    // Component dùng: SuaNXB.razor (Admin - Content & Marketing)
    // File này trước đây hoàn toàn rỗng
    public record UpdatePublisherDto(
        int PublisherId,
        string PublisherName,
        string? Address,
        string? PhoneNumber,
        string? Email,
        string? Website
    );
}
