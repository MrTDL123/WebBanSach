using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using WebApp.Admin.Services.Interfaces;
using WebApp.Shared.Dtos.Management.Auth;

namespace WebApp.Admin.Controllers
{
    [ApiController]
    [Route("api/management")]
    public class AccountController : ControllerBase
    {
        private readonly IAuthClientService _authService;
        public AccountController(IAuthClientService authService)
        {
            _authService = authService;
        }

        [HttpPost("login")]
        [AllowAnonymous]
        [IgnoreAntiforgeryToken]
        public async Task<IActionResult> Login([FromForm] AdminLoginDto dto)
        {
            var result = await _authService.LoginAsync(dto);

            if (!result.Success || result.Data == null)
            {
                var errorMessage = result.Message ?? "Tên đăng nhập hoặc mật khẩu không đúng";
                return BadRequest(new { success = false, message = errorMessage });
            }

            // 3. Nếu ĐĂNG NHẬP THÀNH CÔNG -> Đóng gói thông tin User (Claims)
            var userDto = result.Data;
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
            var principal = new ClaimsPrincipal(identity);
            var authProperties = new AuthenticationProperties
            {
                IsPersistent = true, // Giữ đăng nhập khi tắt trình duyệt
                ExpiresUtc = DateTimeOffset.UtcNow.AddDays(7)
            };

            // 4. Ghi Cookie .WebBanSach.Auth trực tiếp vào Trình duyệt người dùng
            await HttpContext.SignInAsync(IdentityConstants.ApplicationScheme, principal, authProperties);

            return Ok(new { Success = true });
        }

        [HttpGet("logout")]
        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(IdentityConstants.ApplicationScheme);
            return Ok(new { Success = true });
        }
    }
}
