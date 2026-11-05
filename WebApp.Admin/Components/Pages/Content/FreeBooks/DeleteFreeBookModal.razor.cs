using Microsoft.AspNetCore.Components;
using WebApp.Shared.Dtos.Management.ContentAndMarketing;

namespace WebApp.Admin.Components.Pages.Content.FreeBooks
{
    public partial class DeleteFreeBookModal
    {
        private bool _isOpen = false;
        private bool _isDeleting = false;
        private string? _errorMessage;
        private FreeBookListDto? _book;

        [Parameter]
        public EventCallback OnDeleted { get; set; }

        public void Open(FreeBookListDto book)
        {
            _book = book;
            _errorMessage = null;
            _isOpen = true;
            StateHasChanged();
        }

        public void Close()
        {
            _isOpen = false;
            _book = null;
            _errorMessage = null;
        }

        private async Task ExecuteDeleteAsync()
        {
            if (_book == null) return;

            _isDeleting = true;
            _errorMessage = null;

            try
            {
                var result = await ContentClientService.DeleteFreeBookAsync(_book.FreeBookId);
                if (result.Success)
                {
                    Close();
                    await OnDeleted.InvokeAsync();
                }
                else
                {
                    _errorMessage = result.Message ?? "Xóa sách thất bại.";
                }
            }
            catch (Exception ex)
            {
                _errorMessage = "Lỗi hệ thống: " + ex.Message;
            }
            finally
            {
                _isDeleting = false;
            }
        }
    }
}