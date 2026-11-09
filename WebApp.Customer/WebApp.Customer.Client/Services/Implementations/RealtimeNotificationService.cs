using Microsoft.AspNetCore.SignalR.Client;
using WebApp.Customer.Client.Services.Interfaces;

namespace WebApp.Customer.Client.Services.Implementations
{
    public class RealtimeNotificationService : IRealtimeNotificationService
    {
        private HubConnection? _hubConnection;
        private bool _isStarted;

        public event Action<int, int>? OnProductStockUpdated;
        public event Action<int, string>? OnOrderStatusChanged;
        public event Action<string, bool, string?>? OnFeatureToggleChanged;

        public async Task StartAsync()
        {
            // Tránh kết nối lặp lại nếu đã khởi chạy rồi
            if (_isStarted) return;

            // Khởi tạo kết nối trỏ tới UpdateBroadcastHub của WebApp.Api
            _hubConnection = new HubConnectionBuilder()
                .WithUrl("https://localhost:7188/hubs/updates")
                .WithAutomaticReconnect()
                .Build();

            // 1. Đăng ký lắng nghe sự kiện "ReceiveStockUpdate" từ API Hub
            _hubConnection.On<int, int>("ReceiveStockUpdate", (productId, newStock) =>
            {
                OnProductStockUpdated?.Invoke(productId, newStock);
            });

            // 2. Đăng ký lắng nghe sự kiện "ReceiveOrderStatusUpdate" từ API Hub
            _hubConnection.On<int, string>("ReceiveOrderStatusUpdate", (orderId, newStatus) =>
            {
                OnOrderStatusChanged?.Invoke(orderId, newStatus);
            });

            // 3. Đăng ký lắng nghe sự kiện "ReceiveFeatureToggleUpdate" từ API Hub (Feature Toggles)
            _hubConnection.On<string, bool, string?>("ReceiveFeatureToggleUpdate", (key, isEnabled, variant) =>
            {
                OnFeatureToggleChanged?.Invoke(key, isEnabled, variant);
            });

            try
            {
                await _hubConnection.StartAsync();
                _isStarted = true;
            }
            catch
            {
                // Xử lý lỗi nếu API Hub chưa khởi động
            }
        }

        public async ValueTask DisposeAsync()
        {
            if (_hubConnection is not null)
            {
                await _hubConnection.DisposeAsync();
            }
        }
    }
}
