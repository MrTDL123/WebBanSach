using System;
using System.Collections.Generic;
using System.Text;

namespace WebApp.Shared.Dtos.Management.ContentAndMarketing
{
    public record FreeBookDetailDto
    {
        public int FreeBookId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string AuthorName { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? CoverImageUrl { get; set; }
        public int AvailableQty { get; set; }
        public int TotalDonatedQty { get; set; }
        public int TotalRequestedQty { get; set; }
        public string Condition { get; set; } = string.Empty;
        public string? Source { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        // 1. Lịch sử các đợt nhận quyên góp cuốn sách này
        public List<FreeBookDonationHistoryDto> DonationHistories { get; set; } = new();
        // 2. Lịch sử các độc giả xin nhận cuốn sách này
        public List<FreeBookRequestHistoryDto> RequestHistories { get; set; } = new();
    }

    // DTO con: Thông tin người quyên góp sách này
    public record FreeBookDonationHistoryDto(
        int DonationId,
        string DonationCode,
        string DonorName,
        string DonorPhone,
        int QuantityReceived,
        DateTime ReceivedAt
    );

    // DTO con: Thông tin độc giả gửi yêu cầu xin sách này
    public record FreeBookRequestHistoryDto(
        int RequestId,
        string RequestCode,
        string CustomerName,
        string CustomerPhone,
        int QuantityRequested,
        string Reason,             // Lý do xin sách (hoàn cảnh khó khăn, hiếu học...)
        int Status,                // Trạng thái đơn xin (Chờ duyệt, Đã tặng, Đã hủy...)
        DateTime RequestedAt
    );
}
