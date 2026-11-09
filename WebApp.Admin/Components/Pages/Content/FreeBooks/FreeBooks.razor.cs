using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using WebApp.Shared.Dtos.Management.ContentAndMarketing;

namespace WebApp.Admin.Components.Pages.Content.FreeBooks
{
    public partial class FreeBooks
    {
        private FreeBookDetailModal _detailModal = default!;
        private DeleteFreeBookModal _deleteModal = default!;

        private FreeBookFilterDto _filter { get; set; } = new();
        private List<FreeBookListDto> _freeBookList { get; set; } = new();
        private bool _isLoading { get; set; } = true;
        private int _totalItems { get; set; } = 0;
        private int _totalPages => (int)Math.Ceiling((double)_totalItems / _filter.PageSize);
        private int GetDisplayStart() => _totalItems == 0 ? 0 : ((_filter.PageIndex - 1) * _filter.PageSize) + 1;
        private int GetDisplayEnd() => Math.Min(_filter.PageIndex * _filter.PageSize, _totalItems);

        protected override async Task OnInitializedAsync()
        {
            await LoadFreeBooksAsync();
        }

        private async Task LoadFreeBooksAsync()
        {
            _isLoading = true;
            try
            {
                var result = await ContentClientService.GetFreeBooksAsync(_filter);
                if (result.Success && result.Data != null)
                {
                    _freeBookList = result.Data.Items;
                    _totalItems = result.Data.TotalCount;
                }
            }
            catch (Exception) { }
            finally
            {
                _isLoading = false;
            }
        }

        private async Task ApplyFilterAsync()
        {
            _filter.PageIndex = 1;
            await LoadFreeBooksAsync();
        }

        private async Task HandleSearchKeyUp(KeyboardEventArgs e)
        {
            if (e.Key == "Enter")
            {
                await ApplyFilterAsync();
            }
        }

        private async Task OnStatusChanged(ChangeEventArgs e)
        {
            if (bool.TryParse(e.Value?.ToString(), out bool status))
            {
                _filter.IsActive = status;
            }
            else
            {
                _filter.IsActive = null;
            }
            await ApplyFilterAsync();
        }

        private async Task OnStockStatusChanged(ChangeEventArgs e)
        {
            var val = e.Value?.ToString();
            _filter.StockStatus = string.IsNullOrEmpty(val) ? null : val;
            await ApplyFilterAsync();
        }

        private async Task OnSortByChanged(ChangeEventArgs e)
        {
            _filter.SortBy = e.Value?.ToString() ?? "newest";
            await ApplyFilterAsync();
        }

        private async Task ChangePageAsync(int pageIndex)
        {
            if (pageIndex >= 1 && pageIndex <= _totalPages)
            {
                _filter.PageIndex = pageIndex;
                await LoadFreeBooksAsync();
            }
        }

        private async Task OnPageSizeChanged(ChangeEventArgs e)
        {
            if (int.TryParse(e.Value?.ToString(), out int pageSize))
            {
                _filter.PageSize = pageSize;
                _filter.PageIndex = 1;
                await LoadFreeBooksAsync();
            }
        }

        private async Task OpenDetailModal(int freeBookId)
        {
            await _detailModal.OpenAsync(freeBookId);
        }

        private void OpenDeleteModal(FreeBookListDto book)
        {
            _deleteModal.Open(book);
        }

        private string GetImageUrl(string? url)
        {
            if (string.IsNullOrEmpty(url)) return "https://via.placeholder.com/60x80?text=No+Cover";
            return url.StartsWith("http") ? url : $"https://localhost:7188{url}";
        }
    }
}