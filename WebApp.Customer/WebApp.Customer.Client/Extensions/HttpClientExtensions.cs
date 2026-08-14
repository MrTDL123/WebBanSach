using WebApp.Customer.Client.Services.Implementations;
using WebApp.Customer.Client.Services.Interfaces;

namespace WebApp.Customer.Client.Extensions
{
    public static class HttpClientExtensions
    {
        public static IServiceCollection AddApiClientServices<THandler>(
            this IServiceCollection services,
            string apiBaseAddress) where THandler : DelegatingHandler
        {
            // Cấu hình chung cho mọi HttpClient
            void ConfigureDefaultClient(HttpClient client)
            {
                client.BaseAddress = new Uri(apiBaseAddress);
                client.Timeout = TimeSpan.FromSeconds(60);
            }

            // Đăng ký các Service tại đây
            services.AddHttpClient<IAuthClientService, AuthClientService>(ConfigureDefaultClient)
                .AddHttpMessageHandler<THandler>();


            return services;
        }
    }
}
