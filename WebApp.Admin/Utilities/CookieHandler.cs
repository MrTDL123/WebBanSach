namespace WebApp.Admin.Utilities
{
    // Cấu hình Blazor phải nhúng cookie vào mỗi https Request cho bên API đọc
    public class CookieHandler : DelegatingHandler
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        public CookieHandler(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var context = _httpContextAccessor.HttpContext;
            if (context != null && context.Request.Cookies.TryGetValue(".WebBanSach.Auth", out var cookieValue))
            {
                request.Headers.Add("Cookie", $".WebBanSach.Auth={cookieValue}");
            }
            return await base.SendAsync(request, cancellationToken);
        }
    }
}
