using WebApp.Shared.Dtos.Common;

namespace WebApp.Customer.Client.Services.Interfaces
{
    /// <summary>
    /// Service quản lý Feature Toggles (Cờ tính năng).
    /// Cho phép bật/tắt động các khối giao diện (Flash sale, theme theo mùa, banner...)
    /// mà không cần sửa code hay triển khai lại ứng dụng.
    /// </summary>
    public interface IFeatureToggleService
    {
        /// <summary>
        /// Sự kiện phát ra khi cờ tính năng thay đổi (qua SignalR hoặc tải từ API)
        /// </summary>
        event Action? OnFeatureTogglesChanged;

        /// <summary>
        /// Kiểm tra nhanh cờ tính năng có đang bật hay không từ bộ nhớ đệm (O(1))
        /// </summary>
        bool IsEnabled(string flagName, bool defaultValue = false);

        /// <summary>
        /// Lấy giá trị biến thể (variant) của cờ tính năng (Ví dụ: Đại diện cho từng mùa của SeasonalTheme)
        /// </summary>
        string? GetVariant(string flagName);

        /// <summary>
        /// Lấy danh sách toàn bộ các cờ hiện tại
        /// </summary>
        IReadOnlyDictionary<string, FeatureToggleDto> GetAllFlags();

        /// <summary>
        /// Khởi tạo và đồng bộ cờ từ API
        /// </summary>
        Task InitializeAsync();

        /// <summary>
        /// Cập nhật cờ khi nhận được tín hiệu từ SignalR Hub
        /// </summary>
        void UpdateFlag(string key, bool isEnabled, string? variant = null);
    }
}
