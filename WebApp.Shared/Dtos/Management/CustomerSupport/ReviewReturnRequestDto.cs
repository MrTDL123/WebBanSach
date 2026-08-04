using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebBanSach.Shared.Dtos.AdminClient.CustomerSupport
{
    // Component dùng: QuanLyTraHang.razor (Admin - CSKH)
    public record ReviewReturnRequestDto(
        int ReturnRequestId,
        bool IsApproved,
        string? RejectedReason,   // Bắt buộc nếu IsApproved = false
        string? StaffNote
    );
}
