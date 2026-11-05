using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Security.Claims;
using WebApp.Api.Data;
using WebApp.Api.Entities;
using WebApp.Api.Services.Interfaces;
using WebApp.Shared.Dtos.Common;
using WebApp.Shared.Dtos.Customer.Auth;
using WebApp.Shared.Dtos.User;

namespace WebApp.Api.Services.Implementations
{
    public class CustomerAuthService : ICustomerAuthService
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly RoleManager<Role> _roleManager;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IJwtTokenService _jwtTokenService;
        private readonly AppDbContext _dbContext;

        public CustomerAuthService(
            UserManager<User> userManager,
            SignInManager<User> signInManager,
            RoleManager<Role> roleManager,
            IHttpContextAccessor httpContextAccessor,
            IJwtTokenService jwtTokenService,
            AppDbContext dbContext)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _httpContextAccessor = httpContextAccessor;
            _jwtTokenService = jwtTokenService;
            _dbContext = dbContext;
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
                Roles: roles,
                PhoneNumber: user.PhoneNumber,
                DateOfBirth: user.DateOfBirth,
                Address: user.Address
            );

            return ApiResponse<AuthResponseDto>.SuccessResult(authResponse, "Đăng nhập thành công!");
        }

        public async Task<ApiResponse<AuthResponseDto>> LoginCustomerJwtAsync(CustomerLoginDto dto, string? deviceInfo = null)
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

            var roles = (await _userManager.GetRolesAsync(user)).ToList();

            // Sinh Access Token và Refresh Token
            var (accessToken, jwtId, expiresAt) = _jwtTokenService.GenerateAccessToken(user, roles, clientId: "android-app");
            var refreshTokenValue = _jwtTokenService.GenerateRefreshToken();
            var refreshTokenExpiry = DateTime.UtcNow.AddDays(14); // 14 ngày theo cấu hình chuẩn

            var refreshTokenEntity = RefreshToken.Create(
                userId: user.Id,
                token: refreshTokenValue,
                jwtId: jwtId,
                expiresAt: refreshTokenExpiry,
                deviceInfo: deviceInfo
            );

            _dbContext.RefreshTokens.Add(refreshTokenEntity);
            await _dbContext.SaveChangesAsync();

            var authResponse = new AuthResponseDto(
                UserId: user.Id,
                FullName: user.FullName,
                Email: user.Email,
                AvatarUrl: user.AvatarUrl,
                Roles: roles,
                SecurityStamp: user.SecurityStamp,
                AccessToken: accessToken,
                RefreshToken: refreshTokenValue,
                AccessTokenExpiration: expiresAt,
                PhoneNumber: user.PhoneNumber,
                DateOfBirth: user.DateOfBirth,
                Address: user.Address
            );

            return ApiResponse<AuthResponseDto>.SuccessResult(authResponse, "Đăng nhập JWT thành công!");
        }

        public async Task<ApiResponse<AuthResponseDto>> RegisterCustomerAsync(RegisterDto dto, bool isMobile = false, string? deviceInfo = null)
        {
            var existingByEmail = await _userManager.FindByEmailAsync(dto.Email);
            if (existingByEmail != null)
            {
                return ApiResponse<AuthResponseDto>.FailureResult("Email này đã được sử dụng bởi một tài khoản khác.");
            }

            if (!string.IsNullOrWhiteSpace(dto.PhoneNumber))
            {
                var existingByPhone = await _userManager.Users.FirstOrDefaultAsync(u => u.PhoneNumber == dto.PhoneNumber);
                if (existingByPhone != null)
                {
                    return ApiResponse<AuthResponseDto>.FailureResult("Số điện thoại này đã được sử dụng bởi một tài khoản khác.");
                }
            }

            User newUser;
            try
            {
                newUser = User.CreateCustomer(dto.Email, dto.FullName, dto.PhoneNumber);
                if (!string.IsNullOrWhiteSpace(dto.Address))
                {
                    newUser.UpdateProfile(dto.FullName, dto.Address, null);
                }
            }
            catch (ArgumentException ex)
            {
                return ApiResponse<AuthResponseDto>.FailureResult(ex.Message);
            }

            var createResult = await _userManager.CreateAsync(newUser, dto.Password);
            if (!createResult.Succeeded)
            {
                var errors = createResult.Errors.Select(e => e.Description).ToList();
                return ApiResponse<AuthResponseDto>.FailureResult("Đăng ký tài khoản không thành công.", errors);
            }

            const string customerRole = "Customer";
            if (await _roleManager.RoleExistsAsync(customerRole))
            {
                await _userManager.AddToRoleAsync(newUser, customerRole);
            }

            var roles = (await _userManager.GetRolesAsync(newUser)).ToList();

            if (isMobile)
            {
                var (accessToken, jwtId, expiresAt) = _jwtTokenService.GenerateAccessToken(newUser, roles, clientId: "android-app");
                var refreshTokenValue = _jwtTokenService.GenerateRefreshToken();
                var refreshTokenExpiry = DateTime.UtcNow.AddDays(14);

                var refreshTokenEntity = RefreshToken.Create(
                    userId: newUser.Id,
                    token: refreshTokenValue,
                    jwtId: jwtId,
                    expiresAt: refreshTokenExpiry,
                    deviceInfo: deviceInfo
                );

                _dbContext.RefreshTokens.Add(refreshTokenEntity);
                await _dbContext.SaveChangesAsync();

                var authResponse = new AuthResponseDto(
                    UserId: newUser.Id,
                    FullName: newUser.FullName,
                    Email: newUser.Email,
                    AvatarUrl: newUser.AvatarUrl,
                    Roles: roles,
                    SecurityStamp: newUser.SecurityStamp,
                    AccessToken: accessToken,
                    RefreshToken: refreshTokenValue,
                    AccessTokenExpiration: expiresAt,
                    PhoneNumber: newUser.PhoneNumber,
                    DateOfBirth: newUser.DateOfBirth,
                    Address: newUser.Address
                );

                return ApiResponse<AuthResponseDto>.SuccessResult(authResponse, "Đăng ký tài khoản thành công!");
            }
            else
            {
                await WriteAuthCookieAsync(newUser, roles);

                var authResponse = new AuthResponseDto(
                    UserId: newUser.Id,
                    FullName: newUser.FullName,
                    Email: newUser.Email,
                    AvatarUrl: newUser.AvatarUrl,
                    Roles: roles,
                    PhoneNumber: newUser.PhoneNumber,
                    DateOfBirth: newUser.DateOfBirth,
                    Address: newUser.Address
                );

                return ApiResponse<AuthResponseDto>.SuccessResult(authResponse, "Đăng ký tài khoản thành công!");
            }
        }

        public async Task<ApiResponse<AuthResponseDto>> RefreshTokenAsync(RefreshTokenRequestDto dto, string? deviceInfo = null)
        {
            var principal = _jwtTokenService.GetPrincipalFromExpiredToken(dto.AccessToken);
            if (principal == null)
            {
                return ApiResponse<AuthResponseDto>.FailureResult("Access Token không hợp lệ.");
            }

            var userId = principal.FindFirstValue(ClaimTypes.NameIdentifier) ?? principal.FindFirstValue(JwtRegisteredClaimNames.Sub);
            var jwtId = principal.FindFirstValue(JwtRegisteredClaimNames.Jti);

            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(jwtId))
            {
                return ApiResponse<AuthResponseDto>.FailureResult("Token thiếu thông tin định danh.");
            }

            var storedToken = await _dbContext.RefreshTokens
                .FirstOrDefaultAsync(rt => rt.Token == dto.RefreshToken);

            if (storedToken == null)
            {
                return ApiResponse<AuthResponseDto>.FailureResult("Refresh Token không tồn tại.");
            }

            if (storedToken.UserId != userId)
            {
                return ApiResponse<AuthResponseDto>.FailureResult("Refresh Token không khớp với tài khoản.");
            }

            if (storedToken.JwtId != jwtId)
            {
                return ApiResponse<AuthResponseDto>.FailureResult("Refresh Token không khớp với Access Token.");
            }

            if (storedToken.IsRevoked)
            {
                return ApiResponse<AuthResponseDto>.FailureResult("Refresh Token đã bị thu hồi.");
            }

            if (storedToken.IsExpired)
            {
                return ApiResponse<AuthResponseDto>.FailureResult("Refresh Token đã hết hạn.");
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null || !user.IsActive)
            {
                return ApiResponse<AuthResponseDto>.FailureResult("Tài khoản người dùng không hợp lệ hoặc đã bị khóa.");
            }

            var tokenSecurityStamp = principal.FindFirstValue("AspNet.Identity.SecurityStamp");
            if (!string.IsNullOrEmpty(tokenSecurityStamp) && tokenSecurityStamp != user.SecurityStamp)
            {
                return ApiResponse<AuthResponseDto>.FailureResult("Phiên đăng nhập đã hết hiệu lực do tài khoản có thay đổi bảo mật.");
            }

            // Refresh Token Rotation
            var roles = (await _userManager.GetRolesAsync(user)).ToList();
            var (newAccessToken, newJwtId, newExpiresAt) = _jwtTokenService.GenerateAccessToken(user, roles, clientId: "android-app");
            var newRefreshTokenValue = _jwtTokenService.GenerateRefreshToken();
            var newRefreshTokenExpiry = DateTime.UtcNow.AddDays(14);

            storedToken.Revoke(replacedByToken: newRefreshTokenValue);

            var newRefreshTokenEntity = RefreshToken.Create(
                userId: user.Id,
                token: newRefreshTokenValue,
                jwtId: newJwtId,
                expiresAt: newRefreshTokenExpiry,
                deviceInfo: deviceInfo ?? storedToken.DeviceInfo
            );

            _dbContext.RefreshTokens.Add(newRefreshTokenEntity);
            await _dbContext.SaveChangesAsync();

            var authResponse = new AuthResponseDto(
                UserId: user.Id,
                FullName: user.FullName,
                Email: user.Email,
                AvatarUrl: user.AvatarUrl,
                Roles: roles,
                SecurityStamp: user.SecurityStamp,
                AccessToken: newAccessToken,
                RefreshToken: newRefreshTokenValue,
                AccessTokenExpiration: newExpiresAt,
                PhoneNumber: user.PhoneNumber,
                DateOfBirth: user.DateOfBirth,
                Address: user.Address
            );

            return ApiResponse<AuthResponseDto>.SuccessResult(authResponse, "Làm mới Token thành công!");
        }

        public async Task<ApiResponse<bool>> RevokeTokenAsync(string refreshToken, string userId)
        {
            var storedToken = await _dbContext.RefreshTokens
                .FirstOrDefaultAsync(rt => rt.Token == refreshToken && rt.UserId == userId);

            if (storedToken == null)
            {
                return ApiResponse<bool>.FailureResult("Không tìm thấy Refresh Token hợp lệ để thu hồi.");
            }

            if (!storedToken.IsRevoked)
            {
                storedToken.Revoke();
                await _dbContext.SaveChangesAsync();
            }

            return ApiResponse<bool>.SuccessResult(true, "Thu hồi Refresh Token thành công.");
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
                Roles: roles,
                PhoneNumber: user.PhoneNumber,
                DateOfBirth: user.DateOfBirth,
                Address: user.Address
            );

            return ApiResponse<AuthResponseDto>.SuccessResult(userInfo, "Lấy thông tin người dùng thành công.");
        }

        public async Task<ApiResponse<AuthResponseDto>> UpdateProfileAsync(string userId, UpdateProfileDto dto)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null || !user.IsActive)
            {
                return ApiResponse<AuthResponseDto>.FailureResult("Không tìm thấy người dùng hoặc tài khoản đã bị vô hiệu hóa.");
            }

            try
            {
                user.UpdateProfile(dto.FullName, dto.Address, dto.DateOfBirth);
            }
            catch (ArgumentException ex)
            {
                return ApiResponse<AuthResponseDto>.FailureResult(ex.Message);
            }

            // Xử lý lưu avatar nếu có ảnh Base64 được gửi lên
            if (!string.IsNullOrWhiteSpace(dto.AvatarBase64))
            {
                var base64Data = dto.AvatarBase64;
                if (base64Data.Contains(","))
                {
                    base64Data = base64Data.Split(',')[1];
                }

                try
                {
                    var bytes = Convert.FromBase64String(base64Data);
                    if (bytes.Length <= 5 * 1024 * 1024)
                    {
                        var safeFileName = $"avatar_{user.Id}_{Guid.NewGuid():N}.jpg";
                        var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "avatars");
                        if (!Directory.Exists(uploadsFolder))
                        {
                            Directory.CreateDirectory(uploadsFolder);
                        }

                        // Xóa avatar cũ nếu có
                        if (!string.IsNullOrEmpty(user.AvatarUrl))
                        {
                            var oldFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", user.AvatarUrl.TrimStart('/'));
                            if (File.Exists(oldFilePath))
                            {
                                File.Delete(oldFilePath);
                            }
                        }

                        var filePath = Path.Combine(uploadsFolder, safeFileName);
                        await File.WriteAllBytesAsync(filePath, bytes);
                        user.UpdateAvatar($"/images/avatars/{safeFileName}");
                    }
                }
                catch
                {
                    // Bỏ qua nếu có lỗi giải mã ảnh
                }
            }

            var updateResult = await _userManager.UpdateAsync(user);
            if (!updateResult.Succeeded)
            {
                var errors = updateResult.Errors.Select(e => e.Description).ToList();
                return ApiResponse<AuthResponseDto>.FailureResult("Cập nhật thông tin thất bại.", errors);
            }

            var roles = (await _userManager.GetRolesAsync(user)).ToList();

            var authResponse = new AuthResponseDto(
                UserId: user.Id,
                FullName: user.FullName,
                Email: user.Email,
                AvatarUrl: user.AvatarUrl,
                Roles: roles,
                PhoneNumber: user.PhoneNumber,
                DateOfBirth: user.DateOfBirth,
                Address: user.Address
            );

            return ApiResponse<AuthResponseDto>.SuccessResult(authResponse, "Cập nhật hồ sơ thành công!");
        }

        public async Task<ApiResponse<bool>> ChangePasswordAsync(string userId, ChangePasswordDto dto)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null || !user.IsActive)
            {
                return ApiResponse<bool>.FailureResult("Người dùng không tồn tại hoặc tài khoản đã bị khóa.");
            }

            var result = await _userManager.ChangePasswordAsync(user, dto.CurrentPassword, dto.NewPassword);
            if (!result.Succeeded)
            {
                var errors = result.Errors.Select(e => e.Description).ToList();
                return ApiResponse<bool>.FailureResult("Đổi mật khẩu thất bại. Vui lòng kiểm tra lại mật khẩu hiện tại.", errors);
            }

            await _userManager.UpdateSecurityStampAsync(user);

            return ApiResponse<bool>.SuccessResult(true, "Đổi mật khẩu thành công!");
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
