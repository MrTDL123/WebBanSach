using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;
using System.Security.Claims;

namespace WebApp.Admin.Components.Layout
{
    public partial class MainLayout
    {
        // Nhận AuthState được truyền tự động từ <CascadingAuthenticationState> ở App.razor
        [CascadingParameter]
        private Task<AuthenticationState> AuthStateTask { get; set; } = default!;

        [Inject]
        private NavigationManager Navigation { get; set; } = default!;

        private bool isProfileDropdownOpen = false;
        private bool isNotiOpen = false;
        private bool isMobileMenuOpen = false;
        private string userName = "N/A";
        private string userRole = "N/A";
        private string avatarUrl = string.Empty;

        // Lifecycle chuẩn Blazor: Tự động chạy mỗi khi AuthState thay đổi
        protected override async Task OnParametersSetAsync()
        {
            if (AuthStateTask != null)
            {
                var authState = await AuthStateTask;
                ExtractUserInfoFromClaims(authState.User);
            }
        }

        // Trích xuất trực tiếp thông tin từ ClaimsPrincipal đã có sẵn trong RAM
        private void ExtractUserInfoFromClaims(ClaimsPrincipal user)
        {
            if (user.Identity != null && user.Identity.IsAuthenticated)
            {
                userName = user.FindFirst(ClaimTypes.Name)?.Value ?? "Người dùng";
                var roles = user.FindAll(ClaimTypes.Role).Select(r => r.Value).ToList();
                userRole = roles.Any() ? string.Join(", ", roles) : "Nhân viên";
                var avatarClaim = user.FindFirst("AvatarUrl")?.Value;

                if (!string.IsNullOrEmpty(avatarClaim))
                {
                    avatarUrl = avatarClaim.StartsWith("http") ? avatarClaim : $"https://localhost:7188{avatarClaim}";
                }
                else
                {
                    avatarUrl = $"https://ui-avatars.com/api/?name={Uri.EscapeDataString(userName)}&background=E31837&color=fff";
                }
            }
        }

        private async Task HandleLogoutAsync()
        {
            CloseAllDropdowns();
            UserSessionState.Clear(); // Xóa Auth Cookie khỏi bộ nhớ tạm thời trước khi gọi API Logout
            try
            else
            {
                userName = "N/A";
                userRole = "N/A";
                avatarUrl = string.Empty;
            }
        }

        private void ToggleProfileDropdown()
        {
            isProfileDropdownOpen = !isProfileDropdownOpen;
            isNotiOpen = false;
        }

        private void ToggleNotifications()
        {
            isNotiOpen = !isNotiOpen;
            isProfileDropdownOpen = false;
        }

        private void ToggleMobileMenu() => isMobileMenuOpen = !isMobileMenuOpen;

        private void CloseAllDropdowns()
        {
            isProfileDropdownOpen = false;
            isNotiOpen = false;
        }

        private async Task HandleLogoutAsync()
        {
            CloseAllDropdowns();

            try
            {
                // Dùng JS fetch gọi tới AccountController để xóa Cookie ở Trình duyệt
                await JSRuntime.InvokeVoidAsync("fetch", "/api/management/logout", new { method = "POST" });
            }
            catch { }
            Navigation.NavigateTo("/management/login?logout=true", forceLoad: true);
        }
    }
}