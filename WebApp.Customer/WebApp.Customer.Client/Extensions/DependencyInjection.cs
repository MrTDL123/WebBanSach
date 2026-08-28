using FluentValidation;
using WebApp.Customer.Client.Services.Implementations;
using WebApp.Customer.Client.Services.Interfaces;
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
            // Phân quyền Blazor
            services.AddAuthorizationCore();

            // FluentValidation
            services.AddValidatorsFromAssemblyContaining<AssemblyMarker>(lifetime: ServiceLifetime.Singleton);

            // Real time update UI cho các thay đổi từ bên Admin
            services.AddScoped<IRealtimeNotificationService, RealtimeNotificationService>();

            return services;
        }
    }
}
