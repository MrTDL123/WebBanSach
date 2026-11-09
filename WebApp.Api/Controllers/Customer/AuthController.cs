using FluentValidation;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using WebApp.Api.Services.Interfaces;
using WebApp.Shared.Dtos.Common;
using WebApp.Shared.Dtos.Customer.Auth;
using WebApp.Shared.Dtos.User;

namespace WebApp.Api.Controllers.Customer
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly ICustomerAuthService _authService;
        private readonly IValidator<CustomerLoginDto> _loginValidator;
        private readonly IValidator<RefreshTokenRequestDto> _refreshTokenValidator;
        private readonly IValidator<RegisterDto> _registerValidator;
        private readonly IValidator<UpdateProfileDto> _updateProfileValidator;
        private readonly IValidator<ChangePasswordDto> _changePasswordValidator;

        public AuthController(
            ICustomerAuthService authService,
            IValidator<CustomerLoginDto> loginValidator,
            IValidator<RefreshTokenRequestDto> refreshTokenValidator,
            IValidator<RegisterDto> registerValidator,
            IValidator<UpdateProfileDto> updateProfileValidator,
            IValidator<ChangePasswordDto> changePasswordValidator)
        {
            _authService = authService;
            _loginValidator = loginValidator;
            _refreshTokenValidator = refreshTokenValidator;
            _registerValidator = registerValidator;
            _updateProfileValidator = updateProfileValidator;
            _changePasswordValidator = changePasswordValidator;
        }

        /// <summary>
        /// Đăng nhập dành cho Blazor (sử dụng Cookie Authentication)
        /// </summary>
        [HttpPost("customer-login")]
        [AllowAnonymous]
        public async Task<ActionResult<ApiResponse<AuthResponseDto>>> CustomerLogin([FromBody] CustomerLoginDto dto)
        {
            var validationResult = await _loginValidator.ValidateAsync(dto);
            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
                return BadRequest(ApiResponse<AuthResponseDto>.FailureResult("Dữ liệu không hợp lệ.", errors));
            }

            var response = await _authService.LoginCustomerAsync(dto);
            if (!response.Success)
            {
                return BadRequest(response);
            }
            return Ok(response);
        }

        /// <summary>
        /// Đăng nhập dành cho Android Mobile App (trả về Access Token + Refresh Token JWT)
        /// </summary>
        [HttpPost("mobile-login")]
        [AllowAnonymous]
        public async Task<ActionResult<ApiResponse<AuthResponseDto>>> MobileLogin([FromBody] CustomerLoginDto dto)
        {
            var validationResult = await _loginValidator.ValidateAsync(dto);
            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
                return BadRequest(ApiResponse<AuthResponseDto>.FailureResult("Dữ liệu không hợp lệ.", errors));
            }

            var deviceInfo = Request.Headers.UserAgent.ToString();
            var response = await _authService.LoginCustomerJwtAsync(dto, deviceInfo);
            if (!response.Success)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        /// <summary>
        /// Đăng ký tài khoản khách hàng mới (hỗ trợ cả Web Blazor và Mobile Android)
        /// </summary>
        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<ActionResult<ApiResponse<AuthResponseDto>>> Register([FromBody] RegisterDto dto)
        {
            // Kiểm tra xem request đến từ Mobile hay Web (dựa vào header User-Agent hoặc việc có Bearer request)
            var authHeader = Request.Headers.Authorization.ToString();
            var isMobile = Request.Headers.UserAgent.ToString().Contains("okhttp", StringComparison.OrdinalIgnoreCase) ||
                           Request.Headers.UserAgent.ToString().Contains("android", StringComparison.OrdinalIgnoreCase) ||
                           string.IsNullOrWhiteSpace(dto.OtpCode);

            // Bỏ qua lỗi bắt buộc OtpCode nếu là mobile request
            var validationResult = await _registerValidator.ValidateAsync(dto);
            var errors = validationResult.Errors
                .Where(e => !isMobile || e.PropertyName != nameof(dto.OtpCode))
                .Select(e => e.ErrorMessage)
                .ToList();

            if (errors.Any())
            {
                return BadRequest(ApiResponse<AuthResponseDto>.FailureResult("Dữ liệu không hợp lệ.", errors));
            }

            var deviceInfo = Request.Headers.UserAgent.ToString();
            var response = await _authService.RegisterCustomerAsync(dto, isMobile: isMobile, deviceInfo: deviceInfo);
            if (!response.Success)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        /// <summary>
        /// Cấp mới Access Token bằng Refresh Token (hỗ trợ OkHttp Authenticator phía Android)
        /// </summary>
        [HttpPost("refresh-token")]
        [AllowAnonymous]
        public async Task<ActionResult<ApiResponse<AuthResponseDto>>> RefreshToken([FromBody] RefreshTokenRequestDto dto)
        {
            var validationResult = await _refreshTokenValidator.ValidateAsync(dto);
            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
                return BadRequest(ApiResponse<AuthResponseDto>.FailureResult("Dữ liệu không hợp lệ.", errors));
            }

            var deviceInfo = Request.Headers.UserAgent.ToString();
            var response = await _authService.RefreshTokenAsync(dto, deviceInfo);
            if (!response.Success)
            {
                return Unauthorized(response);
            }

            return Ok(response);
        }

        /// <summary>
        /// Thu hồi Refresh Token khi người dùng đăng xuất trên Android App
        /// </summary>
        [HttpPost("revoke-token")]
        [Authorize]
        public async Task<ActionResult<ApiResponse<bool>>> RevokeToken([FromBody] RevokeTokenRequestDto dto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(ApiResponse<bool>.FailureResult("Xác thực không hợp lệ."));
            }

            var response = await _authService.RevokeTokenAsync(dto.RefreshToken, userId);
            if (!response.Success)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        /// <summary>
        /// Lấy thông tin tài khoản hiện tại (hỗ trợ cả Cookie và JWT Bearer)
        /// </summary>
        [HttpGet("me")]
        public async Task<ActionResult<ApiResponse<AuthResponseDto>>> GetCurrentUser()
        {
            if (!User.Identity?.IsAuthenticated ?? true)
            {
                return Unauthorized(ApiResponse<AuthResponseDto>.FailureResult("Chưa đăng nhập."));
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(ApiResponse<AuthResponseDto>.FailureResult("Xác thực không hợp lệ."));
            }

            var response = await _authService.GetCurrentUserAsync(userId);
            if (!response.Success)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        /// <summary>
        /// Cập nhật thông tin cá nhân khách hàng (Hỗ trợ Mobile Android & Web)
        /// </summary>
        [HttpPut("profile")]
        [Authorize]
        public async Task<ActionResult<ApiResponse<AuthResponseDto>>> UpdateProfile([FromBody] UpdateProfileDto dto)
        {
            var validationResult = await _updateProfileValidator.ValidateAsync(dto);
            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
                return BadRequest(ApiResponse<AuthResponseDto>.FailureResult("Dữ liệu không hợp lệ.", errors));
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(ApiResponse<AuthResponseDto>.FailureResult("Xác thực không hợp lệ."));
            }

            var response = await _authService.UpdateProfileAsync(userId, dto);
            if (!response.Success)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        /// <summary>
        /// Đổi mật khẩu tài khoản khách hàng
        /// </summary>
        [HttpPut("change-password")]
        [Authorize]
        public async Task<ActionResult<ApiResponse<bool>>> ChangePassword([FromBody] ChangePasswordDto dto)
        {
            var validationResult = await _changePasswordValidator.ValidateAsync(dto);
            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
                return BadRequest(ApiResponse<bool>.FailureResult("Dữ liệu không hợp lệ.", errors));
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(ApiResponse<bool>.FailureResult("Xác thực không hợp lệ."));
            }

            var response = await _authService.ChangePasswordAsync(userId, dto);
            if (!response.Success)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        /// <summary>
        /// Đăng xuất cho Cookie-based Session (Blazor)
        /// </summary>
        [HttpPost("logout")]
        [Authorize]
        public async Task<ActionResult<ApiResponse<bool>>> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return Ok(ApiResponse<bool>.SuccessResult(true, "Đã đăng xuất thành công."));
        }
    }
}
