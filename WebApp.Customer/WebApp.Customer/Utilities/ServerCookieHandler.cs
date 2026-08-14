namespace WebApp.Customer.Utilities
{
    public class ServerCookieHandler : DelegatingHandler
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        public ServerCookieHandler(IHttpContextAccessor httpContextAccessor)
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
