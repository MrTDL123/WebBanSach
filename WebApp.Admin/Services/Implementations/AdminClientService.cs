using Microsoft.AspNetCore.WebUtilities;
using WebApp.Admin.Services.Base;
using WebApp.Admin.Services.Interfaces;
using WebApp.Admin.Utilities;
using WebApp.Shared.Dtos.Common;
using WebApp.Shared.Dtos.Management.Admin;
using WebApp.Shared.Dtos.Management.ContentAndMarketing;

namespace WebApp.Admin.Services.Implementations
{
    public class AdminClientService : BaseApiClient, IAdminClientService
    {
        private readonly HttpClient _httpClient;

        public AdminClientService(
            HttpClient httpClient,
            UserSessionState userSessionState,
            IHttpContextAccessor httpContextAccessor)
            : base(httpClient, userSessionState, httpContextAccessor)
        {
        }

        public Task<ApiResponse<bool>> BanUserAsync(BanUserDto dto)
        {
            return PostAsJsonAsync<BanUserDto, bool>("api/admin/users/ban", dto);
        }

        public Task<ApiResponse<bool>> CreateStaffAsync(CreateEmployeeDto dto)
        {
            return PostAsJsonAsync<CreateEmployeeDto, bool>("api/admin/staffs", dto);
        }

        public Task<ApiResponse<List<UserListItemDto>>> GetAllStaffAsync()
        {
            return GetAsync<List<UserListItemDto>>("api/admin/staffs");
        }

        public Task<ApiResponse<UserListItemDto>> GetCustomerDetailAsync(string userId)
        {
            return GetAsync<UserListItemDto>($"api/admin/customers/{userId}");
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
            return GetAsync<PagedResultDto<UserListItemDto>>(url);
        }

        public Task<ApiResponse<bool>> UnbanUserAsync(string userId)
        {
            return PostAsync<bool>($"api/admin/users/unban/{userId}");
        }

        public Task<ApiResponse<bool>> UpdateStaffRoleAsync(UpdateEmployeeDto dto)
        {
            return PutAsJsonAsync<UpdateEmployeeDto, bool>("/api/admin/staffs/roles", dto);
        }
    }
}
