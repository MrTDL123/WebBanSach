using WebApp.Admin.Services.Base;
using WebApp.Admin.Services.Interfaces;
using WebApp.Admin.Utilities;
using WebApp.Shared.Dtos.Common;
using WebApp.Shared.Dtos.Management.Common;

namespace WebApp.Admin.Services.Implementations
{
    public class DashboardClientService : BaseApiClient, IDashboardClientService
    {
        public DashboardClientService(
            HttpClient httpClient,
            UserSessionState userSessionState, 
            IHttpContextAccessor httpContextAccessor)
            : base(httpClient, userSessionState, httpContextAccessor)
        {
        }   

        // <summary>
        // Lấy dữ liệu và đưa thông tin lên trang Dashboard
        // </summary>
        public Task<ApiResponse<DashboardStatsDto>> GetDashboardStatsAsync()
        {
            return GetAsync<DashboardStatsDto>("api/management/dashboard/stats");
        }
    }
}
