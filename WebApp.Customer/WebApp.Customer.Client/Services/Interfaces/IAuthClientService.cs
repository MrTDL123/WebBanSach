using WebApp.Shared.Dtos.Common;
using WebApp.Shared.Dtos.Customer.Auth;
namespace WebApp.Customer.Client.Services.Interfaces
{
    public interface IAuthClientService
    {
        Task<ApiResponse<AuthResponseDto>> LoginAsync(CustomerLoginDto dto);
        Task<ApiResponse<AuthResponseDto>> GetCurrentUserAsync();
        Task<ApiResponse<bool>> LogoutAsync();
    }
}
