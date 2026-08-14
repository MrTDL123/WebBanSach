using WebApp.Shared.Dtos.Common;
using WebApp.Shared.Dtos.Customer.Auth;

namespace WebApp.Api.Services.Interfaces
{
    public interface ICustomerAuthService
    {
        Task<ApiResponse<AuthResponseDto>> LoginCustomerAsync(CustomerLoginDto dto);
        Task<ApiResponse<AuthResponseDto>> GetCurrentUserAsync(string userId);
    }
}
