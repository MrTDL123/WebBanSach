using WebApp.Shared.Dtos.CustomerDtos.Product;

namespace WebApp.Customer.Client.Services.Interfaces
{
    /// <summary>
    /// Tải ngầm và lưu tạm thông tin chi tiết sách vào RAM Client khi người dùng rê chuột (Hover) qua thẻ sách.
    /// Giúp chuyển trang chi tiết sách đạt tốc độ tức thì (0ms).
    /// </summary>
    public interface IProductPrefetchService
    {
        void Prefetch(int productId);
        ProductDto? GetCached(int productId);
        void SetCache(int productId, ProductDto product);
    }
}
