using WebApp.Admin.Services.Base;
using WebApp.Admin.Services.Interfaces;
using WebApp.Shared.Dtos.Common;
using WebApp.Shared.Dtos.Management.Common;

namespace WebApp.Admin.Services.Implementations
{
    public class DashboardClientService : BaseApiClient, IDashboardClientService
    {
        private readonly HttpClient _httpClient;
        public DashboardClientService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }   

        // <summary>
        // Lấy dữ liệu và đưa thông tin lên trang Dashboard
        // </summary>
        public Task<ApiResponse<DashboardStatsDto>> GetDashboardStatsAsync()
        {
            return ExecuteApiAsync<DashboardStatsDto>(() => _httpClient.GetAsync("api/management/dashboard/stats"));
        }
    }
}
