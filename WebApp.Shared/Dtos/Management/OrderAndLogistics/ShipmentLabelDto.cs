using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebApp.Shared.Dtos.AdminClient.OrderAndLogistics
{
    // Kết quả trả về sau khi đẩy 3PL thành công để in phiếu gửi (Shipping Label)
    public record ShipmentLabelDto(
        int OrderId,
        string OrderCode,
        string TrackingNumber,         // Mã vận đơn 3PL sinh ra
        string ProviderName,           // Giao Hàng Nhanh / GHTK
        string BarcodeUrl,             // Link ảnh mã vạch / Mã QR vận đơn
        string SenderName,
        string SenderPhone,
        string SenderAddress,
        string ReceiverName,
        string ReceiverPhone,
        string ReceiverAddress,
        decimal CodAmount,             // Số tiền Shipper cần thu
        string ContentNote             // Danh sách sách đóng trong gói
    );
}
