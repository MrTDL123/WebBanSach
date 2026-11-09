using WebApp.Shared.Dtos.Common;
using WebApp.Shared.Dtos.Management.Admin;

namespace WebApp.Api.Services.Interfaces
{
    public interface IAdminService
    {
        Task<ApiResponse<PagedResultDto<UserListItemDto>>> GetCustomersPagedAsync(UserFilterDto filter);
        Task<ApiResponse<List<UserListItemDto>>> GetAllStaffAsync();
        Task<ApiResponse<bool>> CreateStaffAsync(CreateEmployeeDto model);
        Task<ApiResponse<bool>> UpdateStaffRoleAsync(UpdateEmployeeDto model);
        Task<ApiResponse<bool>> BanUserAsync(BanUserDto model);
        Task<ApiResponse<bool>> UnbanUserAsync(string userId);
        Task<ApiResponse<UserListItemDto>> GetCustomerDetailAsync(string userId);
    }
}
