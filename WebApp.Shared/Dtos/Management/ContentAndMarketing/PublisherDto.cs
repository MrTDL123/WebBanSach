namespace WebApp.Shared.Dtos.Management.ContentAndMarketing
{
    // Component dùng: Dropdown NXB (filter), TrangNXB.razor (Customer), QuanLyNXB.razor (Admin)
    public record PublisherDto(
        int PublisherId,
        string PublisherName,
        string? Address,
        string? PhoneNumber,
        int TotalBooks,
        bool IsActive
    );
}
