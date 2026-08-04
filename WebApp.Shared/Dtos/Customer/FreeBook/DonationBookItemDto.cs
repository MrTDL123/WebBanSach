namespace WebBanSach.Shared.Dtos.CustomerDtos.FreeBook
{
    public record DonationBookItemDto(
        string BookTitle,
        string? AuthorName = null,
        int Quantity = 1,
        string? Condition = null,
        string? Note = null
    );
}
