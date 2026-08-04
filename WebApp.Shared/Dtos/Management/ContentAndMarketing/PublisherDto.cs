namespace WebBanSach.Shared.Dtos.AdminClient.ContentAndMarketing
{
    // Component dùng: Dropdown NXB (filter), TrangNXB.razor (Customer), QuanLyNXB.razor (Admin)
    public record PublisherDto(
        int PublisherId,
        string PublisherName,
        string? Address,
        string? PhoneNumber,
        string? Email,
        string? Website,
        int TotalBooks
    );
}
