using Microsoft.Extensions.DependencyInjection;
using WebApp.Customer.Client.Services.Implementations;
using WebApp.Customer.Client.Services.Interfaces;

namespace WebApp.Customer.Client.Extensions
{
    public static class HttpClientExtensions
    {
        /// <summary>
        /// Đăng ký các Typed HttpClient cho WebApp.Customer (cả SSR Pre-rendering và WebAssembly Runtime)
        /// Không dùng DelegatingHandler trung gian để triệt tiêu lỗi DI Scope Mismatch.
        /// </summary>
        public static IServiceCollection AddApiClientServices(
            this IServiceCollection services,
            string apiBaseAddress)
        {
            void ConfigureDefaultClient(HttpClient client)
            {
                client.BaseAddress = new Uri(apiBaseAddress);
                //client.Timeout = TimeSpan.FromSeconds(30);
            }

            // Đăng ký Typed HttpClients cho các dịch vụ
            services.AddHttpClient<IAuthClientService, AuthClientService>(ConfigureDefaultClient);
            services.AddHttpClient<IProductClientService, ProductClientService>(ConfigureDefaultClient);
            services.AddHttpClient<ICartClientService, CartClientService>(ConfigureDefaultClient);
            services.AddHttpClient<IFeatureToggleService, FeatureToggleService>(ConfigureDefaultClient);

            return services;
        }
    }
}
