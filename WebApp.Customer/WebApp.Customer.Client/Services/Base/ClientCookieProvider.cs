namespace WebApp.Customer.Client.Services.Base
{
    /// <summary>
    /// Class lấy cookie trong giai đoạn WebAssembly Runtime.
    /// Trong trình duyệt, cookie được đính kèm tự động nhờ BrowserRequestCredentials.Include.
    /// </summary>
    public class ClientCookieProvider : ICookieProvider
    {
        public string? GetAuthCookie() => null;
    }
}
