using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebApp.Shared.Dtos.AdminClient.WarehouseAndInventory
{
    // ===== BƯỚC 4: XỬ LÝ NHẬP KHO HÀNG HOÀN TRẢ [Role: Warehouse] =====
    // Component dùng: XacNhanHangHoan.razor (Admin - Warehouse)
    public record ReceiveReturnedItemDto(
        int ReturnRequestId,
        int OrderId,
        bool IsGoodCondition,          // True: Sách còn nguyên vẹn -> Nhập lại kho; False: Sách hỏng -> Hủy
        int RestockQuantity,           // Số lượng nhập lại kho
        string? InspectionNote         // Ghi chú kiểm định tình trạng sách
    );
}
