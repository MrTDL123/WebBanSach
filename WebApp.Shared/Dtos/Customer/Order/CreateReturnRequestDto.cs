using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebApp.Shared.Dtos.Customer.Order
{
    // ===== PHIẾU TRẢ HÀNG (RETURN REQUEST) =====
    // Component dùng: YeuCauTraHang.razor (Customer)
    // Khách chỉ được gửi yêu cầu trả hàng khi đơn đang ở trạng thái Completed
    public record CreateReturnRequestDto(
        int OrderId,
        string Reason,              // Bắt buộc: Lý do trả hàng
        string? DetailDescription,  // Mô tả chi tiết vấn đề
                                    // Ảnh minh chứng: Client chuyển file thành Base64 rồi gửi lên API
        List<string> EvidenceImagesBase64,
        List<string> EvidenceImageFileNames
    );
}
