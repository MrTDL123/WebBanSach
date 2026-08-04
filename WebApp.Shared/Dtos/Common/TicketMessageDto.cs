namespace WebBanSach.Shared.Dtos.Common
{
    // Component dùng: TicketDetail.razor (Customer & Admin)
    public record TicketMessageDto(
        int MessageId,
        string SenderName,
        bool IsFromStaff,
        string Content,
        DateTime SentAt
    );
}
