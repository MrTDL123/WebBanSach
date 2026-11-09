using WebApp.Shared.Dtos.Common;
using WebApp.Shared.Dtos.Management.Admin;

namespace WebApp.Admin.Services.Interfaces
{
    public interface IAdminClientService
    {
        Task<ApiResponse<PagedResultDto<UserListItemDto>>> GetCustomersPagedAsync(UserFilterDto filter);
        Task<ApiResponse<List<UserListItemDto>>> GetAllStaffAsync();
        Task<ApiResponse<bool>> CreateStaffAsync(CreateEmployeeDto dto);
        Task<ApiResponse<bool>> UpdateStaffRoleAsync(UpdateEmployeeDto dto);
        Task<ApiResponse<bool>> BanUserAsync(BanUserDto dto);
        Task<ApiResponse<bool>> UnbanUserAsync(string userId);
        Task<ApiResponse<UserListItemDto>> GetCustomerDetailAsync(string userId);

    }
}
