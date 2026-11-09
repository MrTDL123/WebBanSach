using Microsoft.AspNetCore.Components.Forms;
using WebApp.Customer.Client.Services.Base;
using WebApp.Customer.Client.Services.Interfaces;
using WebApp.Shared.Dtos.Common;
using WebApp.Shared.Dtos.Customer.Auth;

namespace WebApp.Customer.Client.Services.Implementations
{
    public class AuthClientService : BaseApiClient, IAuthClientService
    {
        public AuthClientService(
            HttpClient httpClient,
            ICookieProvider? cookieProvider = null,
            AntiforgeryStateProvider? antiforgeryStateProvider = null)
            : base(httpClient, cookieProvider, antiforgeryStateProvider)
        {
        }

        public async Task<ApiResponse<AuthResponseDto>> GetCurrentUserAsync()
        {
            return await GetAsync<AuthResponseDto>("api/auth/me");
        }

        public async Task<ApiResponse<AuthResponseDto>> LoginAsync(CustomerLoginDto dto)
        {
            return await PostAsJsonAsync<CustomerLoginDto, AuthResponseDto>("api/auth/customer-login", dto);
        }

        public async Task<ApiResponse<AuthResponseDto>> RegisterAsync(RegisterDto dto)
        {
            return await PostAsJsonAsync<RegisterDto, AuthResponseDto>("api/auth/register", dto);
        }

        public async Task<ApiResponse<bool>> LogoutAsync()
        {
            return await PostAsync<bool>("api/auth/logout");
        }
    }
}
