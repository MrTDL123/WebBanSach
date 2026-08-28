using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server;

namespace WebApp.Admin.Services
{
    /// <summary>
    /// Class kế thừa cơ chế kiểm tra Auth State sau mỗi chu kỳ nhất định
    /// </summary>
    public class RevalidatingAuthenticationStateProvider : RevalidatingServerAuthenticationStateProvider
    {
        private readonly IServiceScopeFactory _scopeFactory;
        public RevalidatingAuthenticationStateProvider(
            ILoggerFactory loggerFactory,
            IServiceScopeFactory scopeFactory) : base(loggerFactory)
        {
            _scopeFactory = scopeFactory;
        }

        protected override TimeSpan RevalidationInterval => TimeSpan.FromMinutes(5);

        protected override async Task<bool> ValidateAuthenticationStateAsync(
            AuthenticationState authenticationState, CancellationToken cancellationToken)
        {
            var user = authenticationState.User;
            if (user.Identity?.IsAuthenticated != true) return false;

            // Cần lấy class gửi request Service khi pull code của thằng Tiến
            using var scope = _scopeFactory.CreateScope();
            var apiClient = scope.ServiceProvider.GetRequiredService<AdminApiClient>();

            try
            {
                var response = await apiClient.SendAsync(
                new HttpRequestMessage(HttpMethod.Get, "api/auth/validate-stamps"),
                cancellationToken);

                if (!response.IsSuccessStatusCode)
                {
                    // Trả về false -> Blazor Server sẽ hủy Circuit, buộc User đăng nhập lại ngay lập tức
                    return false;
                }

                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
