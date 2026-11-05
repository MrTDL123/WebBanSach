using WebApp.Shared.Enums;

namespace WebApp.Admin.Utilities
{
    public class ToastMessage
    {
        public Guid Id { get; } = Guid.NewGuid();
        public string Title { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public ToastLevel Level { get; set; }
        public DateTime CreatedAt { get; } = DateTime.UtcNow;
    }
}
