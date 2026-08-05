using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebApp.Shared.Dtos.AdminClient.FinanceAndAccountant
{
    // Nhân viên kế toán xác nhận đã nhận tiền COD từ 3PL chuyển về
    public record ReconcileCodDto(
        List<int> OrderIds,           // Tick checkbox nhiều đơn cùng lúc
        string? Note                  // Ghi chú khi đối soát (số phiếu chuyển tiền, batch số...)
    );
}
