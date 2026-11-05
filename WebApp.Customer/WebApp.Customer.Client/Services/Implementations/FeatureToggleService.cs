using System.Collections.Concurrent;
using Microsoft.AspNetCore.Components.Forms;
using WebApp.Customer.Client.Services.Base;
using WebApp.Customer.Client.Services.Interfaces;
using WebApp.Shared.Constants;
using WebApp.Shared.Dtos.Common;

namespace WebApp.Customer.Client.Services.Implementations
{
    public class FeatureToggleService : BaseApiClient, IFeatureToggleService, IDisposable
    {
        private readonly ConcurrentDictionary<string, FeatureToggleDto> _flags = new(StringComparer.OrdinalIgnoreCase);
        private readonly IRealtimeNotificationService _notificationService;
        private bool _isInitialized;

        // Dùng để invoke vẽ lại giao diện dựa vào flag
        public event Action? OnFeatureTogglesChanged;

        public FeatureToggleService(
            HttpClient httpClient,
            IRealtimeNotificationService notificationService,
            ICookieProvider? cookieProvider = null,
            AntiforgeryStateProvider? antiforgeryStateProvider = null)
            : base(httpClient, cookieProvider, antiforgeryStateProvider)
        {
            _notificationService = notificationService;

            // Đăng ký lắng nghe sự kiện Feature Flag từ RealtimeNotificationService
            _notificationService.OnFeatureToggleChanged += HandleRealtimeToggleUpdate;

            // Khởi tạo các giá trị mặc định để giao diện sẵn sàng hiển thị (Graceful Fallback)
            SeedDefaultFlags();
        }

        public bool IsEnabled(string flagName, bool defaultValue = false)
        {
            if (_flags.TryGetValue(flagName, out var flag))
            {
                return flag.IsEnabled;
            }
            return defaultValue;
        }

        public string? GetVariant(string flagName)
        {
            if (_flags.TryGetValue(flagName, out var flag))
            {
                return flag.Variant;
            }
            return null;
        }

        public IReadOnlyDictionary<string, FeatureToggleDto> GetAllFlags()
        {
            return _flags;
        }

        public async Task InitializeAsync()
        {
            if (_isInitialized) return;

            try
            {
                var response = await GetAsync<List<FeatureToggleDto>>("api/features");
                if (response.Success && response.Data != null)
                {
                    foreach (var flag in response.Data)
                    {
                        _flags[flag.Key] = flag;
                    }
                    OnFeatureTogglesChanged?.Invoke();
                }
            }
            catch
            {
                // Giữ các giá trị mặc định nếu API chưa khởi chạy
            }
            finally
            {
                _isInitialized = true;
            }
        }

        public void UpdateFlag(string key, bool isEnabled, string? variant = null)
        {
            _flags.AddOrUpdate(
                key,
                k => new FeatureToggleDto(k, isEnabled, variant),
                (k, existing) => existing with { IsEnabled = isEnabled, Variant = variant }
            );

            OnFeatureTogglesChanged?.Invoke();
        }

        private void HandleRealtimeToggleUpdate(string key, bool isEnabled, string? variant)
        {
            UpdateFlag(key, isEnabled, variant);
        }

        private void SeedDefaultFlags()
        {
            _flags[FeatureFlags.FlashSaleSection] = new FeatureToggleDto(FeatureFlags.FlashSaleSection, true, null, "Khối Flash Sale đếm ngược");
            _flags[FeatureFlags.HeroBannerSlider] = new FeatureToggleDto(FeatureFlags.HeroBannerSlider, true, null, "Banner quảng cáo trang chủ");
            _flags[FeatureFlags.FreeBooksPromotion] = new FeatureToggleDto(FeatureFlags.FreeBooksPromotion, true, null, "Chuyên mục Tủ sách 0đ");
            _flags[FeatureFlags.SeasonalTheme] = new FeatureToggleDto(FeatureFlags.SeasonalTheme, false, "default", "Giao diện theo mùa");
        }

        public void Dispose()
        {
            _notificationService.OnFeatureToggleChanged -= HandleRealtimeToggleUpdate;
        }
    }
}
