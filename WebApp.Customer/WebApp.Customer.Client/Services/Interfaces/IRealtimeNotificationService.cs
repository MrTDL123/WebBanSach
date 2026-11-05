namespace WebApp.Customer.Client.Services.Interfaces
{
    public interface IRealtimeNotificationService : IAsyncDisposable
    {
        /// <summary>
        /// Khởi chạy kết nối WebSocket (Chỉ gọi dưới Browser/WASM)
        /// </summary>
        Task StartAsync();

        /// <summary>
        /// Sự kiện khi Admin cập nhật tồn kho sản phẩm (ProductId, NewStockQuantity)
        /// </summary>
        event Action<int, int>? OnProductStockUpdated;

        /// <summary>
        /// Sự kiện khi Admin thay đổi trạng thái đơn hàng (OrderId, NewStatus)
        /// </summary>
        event Action<int, string>? OnOrderStatusChanged;

        /// <summary>
        /// Sự kiện khi Admin cập nhật cờ tính năng Feature Flag (FlagKey, IsEnabled, Variant)
        /// </summary>
        event Action<string, bool, string?>? OnFeatureToggleChanged;
    }
}
