using System;
using System.Collections.Generic;
using System.Text;

namespace WebApp.Shared.Dtos.Management.ContentAndMarketing
{
    public record FreeBookListDto(
        int FreeBookId,
        string Title,
        string AuthorName,
        string? CoverImageUrl,
        int AvailableQty,          // Số lượng sẵn có trong kho
        int TotalDonatedQty,       // Tổng số lượng đã nhận quyên góp
        int TotalRequestedQty,
        string Condition,          // Tình trạng: "Mới 100%", "Cũ 90% còn tốt"...
        string? Source,            // Nguồn gốc: "Nhà hảo tâm quyên góp", "Quỹ tài trợ"...
        bool IsActive,
        DateTime CreatedAt
    );
}
