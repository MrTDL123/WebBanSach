using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server;
using Microsoft.AspNetCore.Identity;
using System.Runtime.CompilerServices;
using System.Security.Claims;
using WebApp.Admin.Services.Interfaces;
using WebApp.Admin.Utilities;
using WebApp.Shared.Dtos.Common;

namespace WebApp.Admin.Auth
{
    public class CustomAuthStateProvider : RevalidatingServerAuthenticationStateProvider
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly NavigationManager _navigationManager;
        private readonly UserSessionState _circuitSessionState;
        private readonly AuthenticationState _anonymousState = new(new ClaimsPrincipal(new ClaimsIdentity()));

        private AuthenticationState? _cacheAuthState;
        public CustomAuthStateProvider(
            ILoggerFactory loggerFactory,
            IServiceScopeFactory scopeFactory,
            NavigationManager navigationManager,
            UserSessionState circuitSessionState)
            : base(loggerFactory)
        {
            _scopeFactory = scopeFactory;
            _navigationManager = navigationManager;
            _circuitSessionState = circuitSessionState;
        }

        // Chu Kỳ kiểm tra trạng thái xác thực mỗi 5p
        protected override TimeSpan RevalidationInterval => TimeSpan.FromMinutes(10);
        
        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            if (_cacheAuthState != null)
            {
                return _cacheAuthState;
            }

            using var scope = _scopeFactory.CreateScope();

            var childSessionState = scope.ServiceProvider.GetRequiredService<UserSessionState>();
            childSessionState.AuthCookieValue = _circuitSessionState.AuthCookieValue;

            var authService = scope.ServiceProvider.GetRequiredService<IAuthClientService>();
            var response = await authService.GetCurrentUserAsync();

            if (!response.Success || response.Data == null)
            {
                _cacheAuthState = _anonymousState;
                return _anonymousState;
            }

            _cacheAuthState = BuildAuthenticationState(response.Data);
            return _cacheAuthState;
        }

        /// <summary>
        /// Làm mới lại toàn bộ thông tin đăng nhập và quyền hạn từ API
        /// </summary>
        public async Task RefreshAuthenticationStateAsync()
        {
            var authState = await GetAuthenticationStateAsync();
            NotifyAuthenticationStateChanged(Task.FromResult(authState));
        }

        /// <summary>
        /// Thông báo Blazor UI cập nhập lại dựa trạng thái Đăng nhập/Đăng xuất
        /// </summary>
        public void NotifyUserAuthentication(AuthResponseDto userDto)
        {
            _cacheAuthState = BuildAuthenticationState(userDto);
            NotifyAuthenticationStateChanged(Task.FromResult(_cacheAuthState));
        }

        public void NotifyUserLogout()
        {
            _cacheAuthState = _anonymousState;
            NotifyAuthenticationStateChanged(Task.FromResult(_anonymousState));
        }

        protected override async Task<bool> ValidateAuthenticationStateAsync(
            AuthenticationState authenticationState, CancellationToken cancellationToken)
        {
            var user = authenticationState.User;
            if (user.Identity?.IsAuthenticated != true) return false;
             
            // Lấy Security Stamp lấy được lưu trong ClaimsPrincipal 
            var circuitStamp = user.FindFirst("AspNet.Identity.SecurityStamp")?.Value;
            var circuitRoles = user.FindAll(ClaimTypes.Role).Select(r => r.Value).ToList();

            // Hàm CreateScope tạo một vùng scope mới nên UserSessionState lấy từ vùng scope này hoàn toàn mới toanh
            using var scope = _scopeFactory.CreateScope();

            var childSessionState = scope.ServiceProvider.GetRequiredService<UserSessionState>();
            childSessionState.AuthCookieValue = _circuitSessionState.AuthCookieValue;

            var authService = scope.ServiceProvider.GetRequiredService<IAuthClientService>();

            try
            {
                var response = await authService.ValidateSecurityStampAsync();

                if (!response.Success || response.Data == null)
                {
                    TriggerLogout();
                    return false;
                }

                if (!string.IsNullOrEmpty(circuitStamp) && circuitStamp != response.Data.SecurityStamp)
                {
                    TriggerLogout();
                    return false;
                }

                // Kiểm tra toàn bộ các Roles từ 2 danh sách
                var currentDbRoles = response.Data.CurrentRoles ?? new List<string>();
                var circuitRoleSet = new HashSet<string>(circuitRoles, StringComparer.OrdinalIgnoreCase);
                var dbRoleSet = new HashSet<string>(currentDbRoles, StringComparer.OrdinalIgnoreCase);
                
                if (!circuitRoleSet.SetEquals(dbRoleSet))
                {
                    TriggerLogout();
                    return false;
                }

                return true;
            }
            catch
            {
                TriggerLogout();
                return false;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void TriggerLogout()
        {
            _cacheAuthState = _anonymousState;
            NotifyAuthenticationStateChanged(Task.FromResult(_anonymousState));
            _navigationManager.NavigateTo("/api/management/logout?redirect=/management/login", forceLoad: true);
        }

        private AuthenticationState BuildAuthenticationState(AuthResponseDto userDto)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, userDto.UserId ?? ""),
                new Claim(ClaimTypes.Name, userDto.FullName ?? userDto.Email ?? ""),
                new Claim(ClaimTypes.Email, userDto.Email ?? ""),
                new Claim("AvatarUrl", userDto.AvatarUrl ?? ""),
                new Claim("AspNet.Identity.SecurityStamp", userDto.SecurityStamp ?? "")
            };

            if (userDto.Roles != null)
            {
                foreach (var role in userDto.Roles)
                {
                    claims.Add(new Claim(ClaimTypes.Role, role));
                }
            }

            var identity = new ClaimsIdentity(claims, IdentityConstants.ApplicationScheme);
            return new AuthenticationState(new ClaimsPrincipal(identity));
        }
    }
}
