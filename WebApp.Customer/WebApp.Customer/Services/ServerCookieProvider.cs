using Microsoft.AspNetCore.Http;
using WebApp.Customer.Client.Services.Base;

namespace WebApp.Customer.Services
{
    /// <summary>
    /// Class lấy cookie trong giai đoạn Server SSR Pre-rendering.
    /// Trích xuất Cookie ".WebBanSach.Auth" từ IHttpContextAccessor để truyền vào các request API ban đầu.
    /// </summary>
    public class ServerCookieProvider : ICookieProvider
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ServerCookieProvider(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public string? GetAuthCookie()
        {
            var context = _httpContextAccessor.HttpContext;
            if (context != null && context.Request.Cookies.TryGetValue(".WebBanSach.Auth", out var cookieValue))
            {
                return cookieValue;
            }
            return null;
        }
    }
}
