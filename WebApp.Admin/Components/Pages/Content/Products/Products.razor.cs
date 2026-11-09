using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using WebApp.Admin.Services.Implementations;
using WebApp.Shared.Dtos.Management.ContentAndMarketing;

namespace WebApp.Admin.Components.Pages.Content.Products
{
    public partial class Products
    {
        private ProductFilterDto _filter { get; set; } = new();
        private List<ProductListDto> _productList { get; set; } = new();
        private List<CategoryDto> _categories { get; set; } = new();
        private HashSet<int> _selectedProductIds { get; set; } = new();
        private ProductListDto? _productToDelete { get; set; }
        private bool _isDeleting { get; set; }
        private bool _isLoading { get; set; } = true;
        private int _totalItems { get; set; } = 0;
        private int _totalPages => (int)Math.Ceiling((double)_totalItems / _filter.PageSize);
        private bool _isAllSelected => _productList.Any() && _selectedProductIds.Count == _productList.Count;
        private int GetDisplayStart() => _totalItems == 0 ? 0 : ((_filter.PageIndex - 1) * _filter.PageSize) + 1;
        private int GetDisplayEnd() => Math.Min(_filter.PageIndex * _filter.PageSize, _totalItems);

        protected override async Task OnInitializedAsync()
        {
            await LoadCategoriesAsync();
            await LoadProductsAsync();
        }

        private async Task LoadCategoriesAsync()
        {
            try
            {
                var result = await ContentClientService.GetCategoriesAsync();
                if (result.Success && result.Data != null)
                {
                    _categories = result.Data;
                }
            }
            catch(Exception)
            {

            }
        }

        private async Task LoadProductsAsync()
        {
            _isLoading = true;
            try
            {
                var result = await ContentClientService.GetProductsAsync(_filter);

                if (result.Success && result.Data != null)
                {
                    _productList = result.Data.Items;
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
            await LoadProductsAsync();
        }

        private async Task HandleSearchKeyUp(KeyboardEventArgs e)
        {
            if (e.Key == "Enter")
            {
                await ApplyFilterAsync();
            }
        }

        private async Task OnCategoryChanged(ChangeEventArgs e)
        {
            if (int.TryParse(e.Value?.ToString(), out int catId))
            {
                _filter.CategoryId = catId;
            }
            else
            {
                _filter.CategoryId = null;
            }
            await ApplyFilterAsync();
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

        private async Task ChangePageAsync(int pageIndex)
        {
            if (pageIndex >= 1 && pageIndex <= _totalPages)
            {
                _filter.PageIndex = pageIndex;
                await LoadProductsAsync();
            }
        }

        private async Task OnPageSizeChanged(ChangeEventArgs e)
        {
            if (int.TryParse(e.Value?.ToString(), out int pageSize))
            {
                _filter.PageSize = pageSize;
                _filter.PageIndex = 1;
                await LoadProductsAsync();
            }
        }

        private void ToggleSelectAll(ChangeEventArgs e)
        {
            bool check = (bool)(e.Value ?? false);
            if (check)
            {
                _selectedProductIds = _productList.Select(p => p.ProductId).ToHashSet();
            }
            else
            {
                _selectedProductIds.Clear();
            }
        }

        private void ToggleSelectProduct(int productId)
        {
            if (_selectedProductIds.Contains(productId))
            {
                _selectedProductIds.Remove(productId);
            }
            else
            {
                _selectedProductIds.Add(productId);
            }
        }

        private void ConfirmDelete(ProductListDto product)
        {
            _productToDelete = product;
        }

        private void CancelDelete()
        {
            _productToDelete = null;
        }

        private async Task ExecuteDeleteAsync()
        {
            if (_productToDelete == null) return;

            _isDeleting = true;
            try
            {
                var result = await ContentClientService.DeleteProductAsync(_productToDelete.ProductId);
                if (result.Success)
                {
                    _productToDelete = null;
                    await LoadProductsAsync(); // Reload lại danh sách
                }
            }
            catch (Exception) { }
            finally
            {
                _isDeleting = false;
            }
        }

        private string GetImageUrl(string? url)
        {
            if (string.IsNullOrEmpty(url)) return "https://via.placeholder.com/60x80?text=No+Cover";
            return url.StartsWith("http") ? url : $"https://localhost:7188{url}";
        }
    }
}
