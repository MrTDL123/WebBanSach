using WebApp.Shared.Enums;

namespace WebApp.Admin.Utilities
{
    public class ToastHelper
    {
        public event Action<ToastMessage>? OnShow;
        public void Show(ToastLevel level, string message, string? title = null)
        {
            var defaultTitle = level switch
            {
                ToastLevel.Success => "Thành công",
                ToastLevel.Error => "Thất bại",
                ToastLevel.Warning => "Cảnh báo",
                ToastLevel.Info => "Thông báo",
                _ => ""
            };
            OnShow?.Invoke(new ToastMessage
            {
                Level = level,
                Title = title ?? defaultTitle,
                Message = message
            });
        }
        public void ShowSuccess(string message, string? title = null) => Show(ToastLevel.Success, message, title);
        public void ShowError(string message, string? title = null) => Show(ToastLevel.Error, message, title);
        public void ShowWarning(string message, string? title = null) => Show(ToastLevel.Warning, message, title);
        public void ShowInfo(string message, string? title = null) => Show(ToastLevel.Info, message, title);
    }
}
