using FluentValidation;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.JSInterop.Infrastructure;
using System.Security.Claims;
using WebApp.Api.Services.Interfaces;
using WebApp.Shared.Dtos.Common;
using WebApp.Shared.Dtos.Customer.Auth;

namespace WebApp.Api.Controllers.Customer
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly ICustomerAuthService _authService;
        private readonly IValidator<CustomerLoginDto> _loginValidator;

        public AuthController(
            ICustomerAuthService authService,
            IValidator<CustomerLoginDto> loginValidator)
        {
            _authService = authService;
            _loginValidator = loginValidator;
        }

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

        [HttpPost("logout")]
        [Authorize]
        public async Task<ActionResult<ApiResponse<bool>>> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return Ok(ApiResponse<bool>.SuccessResult(true, "Đã đăng xuất thành công."));
        }
    }
}
