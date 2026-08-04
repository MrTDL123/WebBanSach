using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebBanSach.Shared.Dtos.AdminClient.WarehouseAndInventory
{
    public record InventoryReportDto(
        int ProductId,
        string ProductTitle,
        string MainImageUrl,
        int CurrentStock,
        int LowStockThreshold,
        bool IsLowStock
    );
}
