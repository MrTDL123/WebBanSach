namespace WebApp.Admin.Utilities
{
    /// <summary>
    /// Dùng để lưu trữ Auth Cookie trước khi kết nối SignalR
    /// </summary>
    public class UserSessionState
    {
        public string? AuthCookieValue { get; set; }
    }
}
