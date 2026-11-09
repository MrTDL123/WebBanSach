namespace WebApp.Shared.Dtos.Common
{
    /// <summary>
    /// DTO đại diện cho một Feature Flag trong hệ thống
    /// </summary>
    public record FeatureToggleDto(
        string Key,
        bool IsEnabled,
        // Biến thể của một flagcd í dụ: 1 trong 4 mùa riêng biệt của SeasonalTheme)
        string? Variant = null,
        string? Description = null
    );
}
