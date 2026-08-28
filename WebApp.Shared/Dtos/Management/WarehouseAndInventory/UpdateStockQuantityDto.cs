using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebApp.Shared.Dtos.Management.WarehouseAndInventory
{
    // Component dùng: DashboardKho.razor, CapNhatTonKho.razor (Admin - Warehouse)
    public record UpdateStockQuantityDto(
        int ProductId,
        int NewQuantity,
        string Reason
    );
}
