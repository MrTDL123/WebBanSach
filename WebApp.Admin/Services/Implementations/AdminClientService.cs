using Microsoft.AspNetCore.WebUtilities;
using WebApp.Admin.Services.Base;
using WebApp.Admin.Services.Interfaces;
using WebApp.Shared.Dtos.Common;
using WebApp.Shared.Dtos.Management.Admin;
using WebApp.Shared.Dtos.Management.ContentAndMarketing;

namespace WebApp.Admin.Services.Implementations
{
    public class AdminClientService : BaseApiClient, IAdminClientService
    {
        private readonly HttpClient _httpClient;

        public AdminClientService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public Task<ApiResponse<bool>> BanUserAsync(BanUserDto dto)
        {
            return ExecuteApiAsync<bool>(() => _httpClient.PostAsJsonAsync("/api/admin/users/ban", dto));
        }

        public Task<ApiResponse<bool>> CreateStaffAsync(CreateEmployeeDto dto)
        {
            return ExecuteApiAsync<bool>(() => _httpClient.PostAsJsonAsync("/api/admin/staffs", dto));
        }

        public Task<ApiResponse<List<UserListItemDto>>> GetAllStaffAsync()
        {
            return ExecuteApiAsync<List<UserListItemDto>>(() => _httpClient.GetAsync("/api/admin/staffs"));
        }

        public Task<ApiResponse<UserListItemDto>> GetCustomerDetailAsync(string userId)
        {
            return ExecuteApiAsync<UserListItemDto>(() => _httpClient.GetAsync($"/api/admin/customers/{userId}"));
        }

        public Task<ApiResponse<PagedResultDto<UserListItemDto>>> GetCustomersPagedAsync(UserFilterDto filter)
        {
            var queryParams = new Dictionary<string, string?>
            {
                ["searchTerm"] = filter.SearchTerm,
                ["isActive"] = filter.IsActive?.ToString(),
                ["pageIndex"] = filter.PageIndex.ToString(),
                ["pageSize"] = filter.PageSize.ToString()
            };
            string url = QueryHelpers.AddQueryString("/api/admin/customers", queryParams);
            return ExecuteApiAsync<PagedResultDto<UserListItemDto>>(() => _httpClient.GetAsync(url));
        }

        public Task<ApiResponse<bool>> UnbanUserAsync(string userId)
        {
            return ExecuteApiAsync<bool>(() => _httpClient.PostAsync($"/api/admin/users/unban/{userId}", null));
        }

        public Task<ApiResponse<bool>> UpdateStaffRoleAsync(UpdateEmployeeDto dto)
        {
            return ExecuteApiAsync<bool>(() => _httpClient.PutAsJsonAsync("/api/admin/staffs/roles", dto));
        }
    }
}
