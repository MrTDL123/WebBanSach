using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebBanSach.Shared.Dtos.AdminClient.OrderAndLogistics
{
    // ===== BƯỚC 1: KIỂM DUYỆT ĐƠN HÀNG [Role: Order & Logistics] =====
    // Component dùng: QuanLyDonHang.razor (Admin - Order & Logistics)
    public record ApproveOrderDto(
        int OrderId,
        string? Note
    );
}
