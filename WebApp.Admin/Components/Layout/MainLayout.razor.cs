using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Mvc.TagHelpers.Cache;
using Microsoft.JSInterop;
using System.Security.Claims;
using WebApp.Admin.Auth;
using WebApp.Admin.Services.Implementations;

namespace WebApp.Admin.Components.Layout
{
    public partial class MainLayout : IDisposable
    {
        private bool isProfileDropdownOpen = false;
        private bool isNotiOpen = false;
        private bool isMobileMenuOpen = false;
        private string userName = "N/A";
        private string userRole = "N/A";
        private string avatarUrl = string.Empty;

        protected override async Task OnInitializedAsync()
        {
            // Đăng ký lắng nghe sự kiện khi thông tin User thay đổi (ví dụ: Profile.razor vừa lưu Avatar/Tên mới).
            // Phải tuân theo cú pháp tham số (Task<AuthenticationState> task) do Microsoft C# định nghĩa sẵn.
            AuthStateProvider.AuthenticationStateChanged += OnAuthStateChanged;
            await LoadUserInfoFromClaimsAsync();
        }

        private async void OnAuthStateChanged(Task<AuthenticationState> task)
        {
            await LoadUserInfoFromClaimsAsync();
            /*
             * Vì Profile và MainLayout là 2 giao diện độc lập, sự kiện bắn sang là sự kiện ngầm (Background/Async),
                nên bắt buộc phải dùng 'InvokeAsync' để chuyển lệnh 'StateHasChanged' về luồng giao diện chính an toàn,
                yêu cầu Blazor vẽ lại Avatar và Tên ở góc phải màn hình ngay lập tức. 
            */
            await InvokeAsync(StateHasChanged);
        }

        private async Task LoadUserInfoFromClaimsAsync()
        {
            var authState = await AuthStateProvider.GetAuthenticationStateAsync();
            var user = authState.User;

            if (user.Identity != null && user.Identity.IsAuthenticated)
            {
                userName = user.FindFirst(ClaimTypes.Name)?.Value ?? "Người dùng";
                var roles = user.FindAll(ClaimTypes.Role).Select(r => r.Value).ToList();
                userRole = roles.Any() ? string.Join(", ", roles) : "Nhân viên";
                var avatarClaim = user.FindFirst("AvatarUrl")?.Value;

                if (!string.IsNullOrEmpty(avatarClaim))
                {
                    // Nối Domain API nếu là đường dẫn tương đối /images/avatars/...
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

            try
            {
                // 1. Dùng JS fetch gọi tới AccountController để xóa Cookie ở Trình duyệt
                await JSRuntime.InvokeVoidAsync("fetch", "/api/management/logout", new { method = "POST" });
            }
            catch { }
            Navigation.NavigateTo("/management/login?logout=true", forceLoad: true);
        }

        public void Dispose()
        {
            // Hủy đăng ký sự kiện khi Component bị hủy để tránh rò rỉ bộ nhớ
            AuthStateProvider.AuthenticationStateChanged -= OnAuthStateChanged;
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

        // Hàm dùng để đóng tất cả các dropdown khi bấm ra ngoài hoặc click chọn menu item
        private void CloseAllDropdowns()
        {
            isProfileDropdownOpen = false;
            isNotiOpen = false;
        }
    }
}
