using Microsoft.AspNetCore.Components;
using WebApp.Shared.Dtos.Management.ContentAndMarketing;

namespace WebApp.Admin.Components.Pages.Content.FreeBooks
{
    public partial class FreeBookDetailModal
    {
        private bool _isOpen = false;
        private bool _isLoading = false;
        private string? _errorMessage;
        private FreeBookDetailDto? _detailBook;

        public async Task OpenAsync(int freeBookId)
        {
            _isOpen = true;
            _isLoading = true;
            _errorMessage = null;
            StateHasChanged();

            try
            {
                var result = await ContentClientService.GetFreeBookDetailAsync(freeBookId);
                if (result.Success && result.Data != null)
                {
                    _detailBook = result.Data;
                }
                else
                {
                    _errorMessage = result.Message ?? "Không thể lấy thông tin chi tiết.";
                }
            }
            catch (Exception ex)
            {
                _errorMessage = "Lỗi hệ thống: " + ex.Message;
            }
            finally
            {
                _isLoading = false;
                StateHasChanged();
            }
        }

        public void Close()
        {
            _isOpen = false;
            _detailBook = null;
            _errorMessage = null;
        }

        private string GetRequestStatusLabel(int status) => status switch
        {
            0 => "Chờ duyệt",
            1 => "Đã tặng",
            2 => "Đã hủy",
            _ => "Không rõ"
        };

        private string GetImageUrl(string? url)
        {
            if (string.IsNullOrEmpty(url)) return "https://via.placeholder.com/100x136?text=No+Cover";
            return url.StartsWith("http") ? url : $"https://localhost:7188{url}";
        }
    }
}