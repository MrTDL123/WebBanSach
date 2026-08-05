using WebApp.Shared.Enums;

namespace WebApp.Shared.Dtos.CustomerDtos.Order
{
    // API trả về thông tin một phiếu trả hàng để hiển thị
    public record ReturnRequestDto(
        int ReturnRequestId,
        int OrderId,
        string OrderCode,
        ReturnStatus Status,
        string Reason,
        string? DetailDescription,
        List<string> EvidenceImageUrls,
        DateTime CreatedAt,
        // Thông tin xử lý (nếu đã được duyệt)
        string? ReviewedByName,
        DateTime? ReviewedAt,
        string? RejectedReason
    );
}
