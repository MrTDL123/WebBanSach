using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using WebApp.Api.Entities;
using WebApp.Api.Services.Interfaces;
using WebApp.Api.Utilities;

namespace WebApp.Api.Services.Implementations
{
    /// <summary>
    /// Service triển khai sinh và xác thực JWT token theo chuẩn Microsoft Security & OAuth 2.0
    /// </summary>
    public class JwtTokenService : IJwtTokenService
    {
        private readonly JwtSettings _jwtSettings;
        private readonly byte[] _keyBytes;

        public JwtTokenService(IOptions<JwtSettings> jwtOptions)
        {
            _jwtSettings = jwtOptions.Value;
            if (string.IsNullOrWhiteSpace(_jwtSettings.SecretKey) || _jwtSettings.SecretKey.Length < 32)
            {
                throw new InvalidOperationException("JwtSettings:SecretKey phải có độ dài tối thiểu 256 bits (32 ký tự).");
            }
            _keyBytes = Encoding.UTF8.GetBytes(_jwtSettings.SecretKey);
        }

        public (string Token, string JwtId, DateTime ExpiresAt) GenerateAccessToken(User user, IList<string> roles, string clientId = "android-app")
        {
            var jwtId = Guid.NewGuid().ToString();
            var expiresAt = DateTime.UtcNow.AddMinutes(_jwtSettings.AccessTokenExpirationMinutes);

            var claims = new List<Claim>
            {
                // Chuẩn OAuth 2.0 / OpenID Connect claims
                new Claim(JwtRegisteredClaimNames.Sub, user.Id),
                new Claim(JwtRegisteredClaimNames.Jti, jwtId),
                new Claim("client_id", clientId),

                // Claims tương thích với ASP.NET Core Identity & CurrentUserService
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Name, user.FullName ?? user.UserName ?? user.Email ?? string.Empty),
                new Claim(ClaimTypes.Email, user.Email ?? string.Empty),
                new Claim("AspNet.Identity.SecurityStamp", user.SecurityStamp ?? string.Empty)
            };

            // Thêm Role claims
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var signingKey = new SymmetricSecurityKey(_keyBytes);
            var signingCredentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Issuer = _jwtSettings.Issuer,
                Audience = _jwtSettings.Audience,
                IssuedAt = DateTime.UtcNow,
                NotBefore = DateTime.UtcNow,
                Expires = expiresAt,
                SigningCredentials = signingCredentials
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var securityToken = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(securityToken);

            return (tokenString, jwtId, expiresAt);
        }

        public string GenerateRefreshToken()
        {
            var randomBytes = RandomNumberGenerator.GetBytes(64);
            return Convert.ToBase64String(randomBytes);
        }

        public ClaimsPrincipal? GetPrincipalFromExpiredToken(string token)
        {
            if (string.IsNullOrWhiteSpace(token))
                return null;

            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = _jwtSettings.Issuer,
                ValidateAudience = true,
                ValidAudience = _jwtSettings.Audience,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(_keyBytes),
                ValidateLifetime = false, // Cho phép đọc claims ngay cả khi token đã hết hạn (expired)
                ClockSkew = TimeSpan.Zero
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            try
            {
                var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out var securityToken);

                // Kiểm tra nghiêm ngặt xem token có phải là JwtSecurityToken sử dụng thuật toán HmacSha256 hay không
                if (securityToken is not JwtSecurityToken jwtSecurityToken ||
                    !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
                {
                    return null;
                }

                return principal;
            }
            catch
            {
                return null;
            }
        }
    }
}
