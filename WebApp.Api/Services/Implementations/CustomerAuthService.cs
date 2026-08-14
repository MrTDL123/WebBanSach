using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using WebApp.Api.Entities;
using WebApp.Api.Services.Interfaces;
using WebApp.Shared.Dtos.Common;
using WebApp.Shared.Dtos.Customer.Auth;

namespace WebApp.Api.Services.Implementations
{
    public class CustomerAuthService : ICustomerAuthService
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CustomerAuthService(
            UserManager<User> userManager,
            SignInManager<User> signInManager,
            IHttpContextAccessor httpContextAccessor)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _httpContextAccessor = httpContextAccessor;
        }
        public async Task<ApiResponse<AuthResponseDto>> LoginCustomerAsync(CustomerLoginDto dto)
        {
            var user = await _userManager.Users
                .FirstOrDefaultAsync(u => u.Email == dto.EmailOrPhone || u.PhoneNumber == dto.EmailOrPhone);

            if (user == null)
            {
                return ApiResponse<AuthResponseDto>.FailureResult("Tài khoản hoặc mật khẩu không chính xác.");
            }

            try
            {
                user.EnsureCanLogin();
            }
            catch (InvalidOperationException ex)
            {
                return ApiResponse<AuthResponseDto>.FailureResult(ex.Message);
            }

            var signInResult = await _signInManager.CheckPasswordSignInAsync(user, dto.Password, lockoutOnFailure: true);

            if (signInResult.IsLockedOut)
            {
                var lockoutEnd = await _userManager.GetLockoutEndDateAsync(user);
                var remainingMinutes = lockoutEnd.HasValue
                    ? Math.Max(1, (int)Math.Ceiling((lockoutEnd.Value - DateTimeOffset.UtcNow).TotalMinutes))
                    : 15;

                return ApiResponse<AuthResponseDto>.FailureResult($"Tài khoản đã bị tạm khóa do nhập sai mật khẩu 5 lần liên tiếp. Vui lòng thử lại sau {remainingMinutes} phút.");
            }

            if (!signInResult.Succeeded)
            {
                var accessFailedCount = await _userManager.GetAccessFailedCountAsync(user);
                var remainingAttempts = 5 - accessFailedCount;

                string warningMsg = remainingAttempts > 0
                    ? $"Tài khoản hoặc mật khẩu không chính xác. Bạn còn {remainingAttempts} lần thử trước khi tài khoản bị khóa."
                    : "Tài khoản hoặc mật khẩu không chính xác.";

                return ApiResponse<AuthResponseDto>.FailureResult(warningMsg);
            }

            // Đăng nhập thành công
            try
            {
                user.RecordLoginSuccess();
                await _userManager.UpdateAsync(user);
            }
            catch (InvalidOperationException ex)
            {
                return ApiResponse<AuthResponseDto>.FailureResult(ex.Message);
            }

            // Lấy role của user hiện tại và lưu vào cookie
            var roles = (await _userManager.GetRolesAsync(user)).ToList();

            // Đọc các claims và thêm cookie được mã hóa vào Header Set-Cookie
            await WriteAuthCookieAsync(user, roles);

            var authResponse = new AuthResponseDto(
                UserId: user.Id,
                FullName: user.FullName,
                Email: user.Email,
                AvatarUrl: user.AvatarUrl,
                Roles: roles
            );

            return ApiResponse<AuthResponseDto>.SuccessResult(authResponse, "Đăng nhập thành công!");
        }

        public async Task<ApiResponse<AuthResponseDto>> GetCurrentUserAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null || !user.IsActive)
            {
                return ApiResponse<AuthResponseDto>.FailureResult("Không tìm thấy người dùng hoặc tài khoản đã bị khóa.");
            }

            var roles = (await _userManager.GetRolesAsync(user)).ToList();

            var userInfo = new AuthResponseDto(
                UserId: user.Id,
                FullName: user.FullName ?? user.UserName ?? "Khách hàng",
                Email: user.Email ?? "",
                AvatarUrl: user.AvatarUrl,
                Roles: roles
            );

            return ApiResponse<AuthResponseDto>.SuccessResult(userInfo, "Lấy thông tin người dùng thành công.");
        }

        private async Task WriteAuthCookieAsync(User user, List<string> roles)
        {
            var httpContext = _httpContextAccessor.HttpContext;
            if (httpContext == null) return;

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Name, user.FullName ?? user.UserName ?? ""),
                new Claim(ClaimTypes.Email, user.Email ?? "")
            };

            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var claimsIdentity = new ClaimsIdentity(claims, IdentityConstants.ApplicationScheme);
            var authProperties = new AuthenticationProperties
            {
                IsPersistent = true,
                ExpiresUtc = DateTimeOffset.UtcNow.AddDays(7)
            };

            await httpContext.SignInAsync(
                IdentityConstants.ApplicationScheme,
                new ClaimsPrincipal(claimsIdentity),
                authProperties
            );
        }
    }
}
