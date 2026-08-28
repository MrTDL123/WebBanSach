using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.RenderTree;
using WebApp.Customer.Client.Services.Implementations;
using WebApp.Customer.Client.Services.Interfaces;
using WebApp.Customer.Client.Utilities;

namespace WebApp.Customer.Client.Extensions
{
    public static class HttpClientExtensions
    {
        public static IServiceCollection AddApiClientServices<THandler>(
            this IServiceCollection services,
            string apiBaseAddress,
            bool isAssemblyRenderMode) where THandler : DelegatingHandler
        {
            // Cấu hình chung cho mọi HttpClient
            void ConfigureDefaultClient(HttpClient client)
            {
                client.BaseAddress = new Uri(apiBaseAddress);
                //client.Timeout = TimeSpan.FromSeconds(60);
            }

            // Đăng ký các Service tại đây
            var httpClient = services.AddHttpClient<IAuthClientService, AuthClientService>(ConfigureDefaultClient)
                            .AddHttpMessageHandler<THandler>();
            // Chỉ thêm vào render mode Wasm để tránh cấu hình CORS sai
            if (isAssemblyRenderMode)
            {
                httpClient.AddHttpMessageHandler<AntiforgeryHandler>();
            }

            return services;
        }
    }
}
