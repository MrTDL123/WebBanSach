namespace WebBanSach.Shared.Dtos.Common
{
    // Component dùng: LienHe.razor (Customer - Form tạo ticket hỗ trợ)
    public record CreateSupportTicketDto(
        string Subject,
        string Message,
        int? RelatedOrderId = null
    );
}
