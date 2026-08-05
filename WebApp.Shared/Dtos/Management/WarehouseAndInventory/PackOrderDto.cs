using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebApp.Shared.Dtos.AdminClient.WarehouseAndInventory
{
    // ===== BƯỚC 2: ĐÓNG GÓI SÁCH [Role: Warehouse & Inventory] =====
    // Component dùng: DonHangChoDongGoi.razor (Admin - Warehouse)
    public record PackOrderDto(
        int OrderId,
        string WarehouseStaffId,
        string? PackageNote
    );
}
