using WebApp.Admin.Utilities;

namespace WebApp.Admin.Components.Layout
{
    public partial class ToastContainer
    {
        private readonly List<ToastMessage> _toasts = new();

        protected override void OnInitialized()
        {
            Toast.OnShow += HandleShowToast;
        }

        private void HandleShowToast(ToastMessage toast)
        {
            InvokeAsync(async () =>
            {
                _toasts.Add(toast);
                StateHasChanged();

                await Task.Delay(3500);
                RemoveToast(toast.Id);
            });
        }

        private void RemoveToast(Guid id)
        {
            InvokeAsync(() =>
            {
                var item = _toasts.FirstOrDefault(t => t.Id == id);
                if (item != null)
                {
                    _toasts.Remove(item);
                    StateHasChanged();
                }
            });
        }

        public void Dispose()
        {
            Toast.OnShow -= HandleShowToast;
        }
    }
}
