using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using WebApp.Customer.Client.Services.Implementations;
using WebApp.Customer.Client.Services.Interfaces;
using WebApp.Customer.Client.States;
using WebApp.Shared;

namespace WebApp.Customer.Client.Extensions
{
    public static class DependencyInjection
    {
        /// <summary>
        /// Extension method đăng ký toàn bộ Client Services dùng chung cho cả Server Prerender và WASM Runtime.
        /// </summary>
        public static IServiceCollection AddSharedClientServices(this IServiceCollection services)
        {
            // FluentValidation
            services.AddValidatorsFromAssemblyContaining<AssemblyMarker>(lifetime: ServiceLifetime.Singleton);

            // Real-time SignalR service nhận thông báo cập nhật từ WebApp.Api
            services.AddScoped<IRealtimeNotificationService, RealtimeNotificationService>();

            // Quản lý trạng thái giỏ hàng C# Client
            services.AddScoped<CartState>();

            // Load dữ liệu ngầm khi khách hàng hover
            services.AddSingleton<IProductPrefetchService, ProductPrefetchService>();

            return services;
        }
    }
}
