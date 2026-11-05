using System.Collections.Concurrent;
using Microsoft.Extensions.DependencyInjection;
using WebApp.Customer.Client.Services.Interfaces;
using WebApp.Shared.Dtos.CustomerDtos.Product;

namespace WebApp.Customer.Client.Services.Implementations
{
    public class ProductPrefetchService : IProductPrefetchService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ConcurrentDictionary<int, ProductDto> _cache = new();
        private readonly ConcurrentDictionary<int, bool> _inFlight = new();

        public ProductPrefetchService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public void Prefetch(int productId)
        {
            if (productId <= 0 || _cache.ContainsKey(productId) || _inFlight.ContainsKey(productId))
            {
                return;
            }

            _inFlight.TryAdd(productId, true);

            _ = Task.Run(async () =>
            {
                try
                {
                    using var scope = _serviceProvider.CreateScope();
                    var productService = scope.ServiceProvider.GetService<IProductClientService>();
                    if (productService != null)
                    {
                        var response = await productService.GetProductByIdAsync(productId);
                        if (response.Success && response.Data != null)
                        {
                            _cache.TryAdd(productId, response.Data);
                        }
                    }
                }
                catch
                {
                    // Lỗi mạng hoặc API chưa bật không ảnh hưởng đến trải nghiệm người dùng
                }
                finally
                {
                    _inFlight.TryRemove(productId, out _);
                }
            });
        }

        public ProductDto? GetCached(int productId)
        {
            _cache.TryGetValue(productId, out var product);
            return product;
        }

        public void SetCache(int productId, ProductDto product)
        {
            _cache[productId] = product;
        }
    }
}
