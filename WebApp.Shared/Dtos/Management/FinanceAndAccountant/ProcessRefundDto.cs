using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebBanSach.Shared.Dtos.AdminClient.FinanceAndAccountant
{
    // ===== BƯỚC 5: XỬ LÝ HOÀN TIỀN [Role: Finance & Accountant] =====
    // Component dùng: QuanLyHoanTien.razor (Admin - Finance)
    public record ProcessRefundDto(
        int ReturnRequestId,
        int OrderId,
        decimal RefundAmount,
        string TransactionReference,   // Mã giao dịch ngân hàng chuyển tiền lại cho khách
        string? Note
    );
}
