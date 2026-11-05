using System;

namespace WebApp.Api.Entities;

/// <summary>
/// Thực thể RefreshToken quản lý phiên đăng nhập JWT của người dùng (DDD/Rich Domain Model)
/// </summary>
public partial class RefreshToken
{
    public int Id { get; private set; }
    public string UserId { get; private set; } = null!;
    public string Token { get; private set; } = null!;
    public string JwtId { get; private set; } = null!; // Liên kết với claim 'jti' của Access Token
    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
    public DateTime ExpiresAt { get; private set; }
    public DateTime? RevokedAt { get; private set; }
    public string? ReplacedByToken { get; private set; }
    public string? DeviceInfo { get; private set; }

    // Navigation property
    public virtual User User { get; private set; } = null!;

    // Computed properties
    public bool IsExpired => DateTime.UtcNow >= ExpiresAt;
    public bool IsRevoked => RevokedAt != null;
    public bool IsActive => !IsRevoked && !IsExpired;

    // EF Core constructor
    protected RefreshToken() { }

    public static RefreshToken Create(
        string userId,
        string token,
        string jwtId,
        DateTime expiresAt,
        string? deviceInfo = null)
    {
        if (string.IsNullOrWhiteSpace(userId))
            throw new ArgumentException("UserId không được để trống", nameof(userId));
        if (string.IsNullOrWhiteSpace(token))
            throw new ArgumentException("Token không được để trống", nameof(token));
        if (string.IsNullOrWhiteSpace(jwtId))
            throw new ArgumentException("JwtId không được để trống", nameof(jwtId));
        if (expiresAt <= DateTime.UtcNow)
            throw new ArgumentException("Hạn dùng Token phải lớn hơn thời gian hiện tại", nameof(expiresAt));

        return new RefreshToken
        {
            UserId = userId,
            Token = token,
            JwtId = jwtId,
            CreatedAt = DateTime.UtcNow,
            ExpiresAt = expiresAt,
            DeviceInfo = deviceInfo
        };
    }

    /// <summary>
    /// Thu hồi token
    /// </summary>
    public void Revoke(string? replacedByToken = null)
    {
        if (IsRevoked)
            return;

        RevokedAt = DateTime.UtcNow;
        ReplacedByToken = replacedByToken;
    }
}
