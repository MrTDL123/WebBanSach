using WebApp.Shared.Dtos.Common;
using WebApp.Shared.Dtos.Management.Common;

namespace WebApp.Admin.Services.Interfaces
{
    public interface IDashboardClientService
    {
        /// <summary>
        /// Lấy thông tin thống kê cho Dashboard Admin
        /// </summary>
        Task<ApiResponse<DashboardStatsDto>> GetDashboardStatsAsync();
    }
}
