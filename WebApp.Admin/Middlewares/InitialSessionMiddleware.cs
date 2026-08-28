using WebApp.Admin.Utilities;

namespace WebApp.Admin.Middlewares
{
    /// <summary>
    /// Lưu Cookie vào lần load SSR đầu tiên trước khi kết nối SignalR
    /// </summary>
    public class InitialSessionMiddleware
    {
        private readonly RequestDelegate _next;

        public InitialSessionMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, UserSessionState sessionState)
        {
            // Bắt cookie từ giai đoạn load Static SSR và nạp vào UserSession cho đồng bộ vòng đời của một tab trong trình duyệt  
            if (context.Request.Cookies.TryGetValue(".WebBanSach.Auth", out var cookieVal))
            {
                sessionState.AuthCookieValue = cookieVal;
            }

            await _next(context);
        }
    }
}
