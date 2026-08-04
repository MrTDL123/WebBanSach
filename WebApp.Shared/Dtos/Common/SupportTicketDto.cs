namespace WebBanSach.Shared.Dtos.Common
{
    // API trả về thông tin ticket để hiển thị cho cả Customer lẫn Admin
    public record SupportTicketDto(
        int TicketId,
        string TicketCode,
        string Subject,
        string CustomerName,
        string? CustomerEmail,
        int Status,
        string StatusText,
        DateTime CreatedAt,
        DateTime? LastReplyAt,
        int? RelatedOrderId,
        List<TicketMessageDto> Messages
    );
}
