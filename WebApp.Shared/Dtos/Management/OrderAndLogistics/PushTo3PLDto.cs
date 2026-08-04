using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebBanSach.Shared.Dtos.AdminClient.OrderAndLogistics
{
    // ===== BƯỚC 3: ĐẨY ĐƠN SANG 3PL [Role: Order & Logistics] =====
    // Component dùng: QuanLyDonHang.razor (Admin - Order & Logistics)
    // Kích hoạt API kết nối sang GHN / GHTK để lấy mã vận đơn tự động
    public record PushTo3PLDto(
        int OrderId,
        int ShippingProviderId,       // 1: GHN, 2: GHTK, 3: ViettelPost
        decimal TotalWeightGram,      // Khối lượng tính phí ship
        string PickUpAddressId,       // Kho lấy hàng
        string? NoteForShipper        // Ghi chú cho Shipper (VD: "Cho xem hàng, không thử")
    );
}
