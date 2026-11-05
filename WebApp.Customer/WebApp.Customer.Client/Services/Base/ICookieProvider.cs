namespace WebApp.Customer.Client.Services.Base
{
    /// <summary>
    /// Giúp tương thích liền mạch giữa Server SSR Pre-rendering (dùng IHttpContextAccessor)
    /// và Client WebAssembly (trình duyệt tự gắn cookie).
    /// </summary>
    public interface ICookieProvider
    {
        string? GetAuthCookie();
    }
}
