using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebBanSach.Shared.Dtos.AdminClient.OrderAndLogistics
{
    // Component dùng: QuanLyDonHang.razor (Admin - Order & Logistics)
    public record UpdateOrderStatusDto(
        int OrderId,
        int NewStatus,
        string? TrackingNumber = null,
        int? ShippingProviderId = null,
        string? Note = null
    );
}
