using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using WebApp.Shared.Dtos.Management.ContentAndMarketing;

namespace WebApp.Admin.Components.Pages.Content
{
    public partial class Products
    {
        private ProductFilterDto Filter { get; set; } = new();
        private List<ProductListDto> ProductList { get; set; } = new();
        private List<CategoryDto> Categories { get; set; } = new();
        private HashSet<int> SelectedProductIds { get; set; } = new();

        private bool IsLoading { get; set; } = true;
        private int TotalItems { get; set; } = 0;
        private int TotalPages => (int)Math.Ceiling((double)TotalItems / Filter.PageSize);
        private bool IsAllSelected => ProductList.Any() && SelectedProductIds.Count == ProductList.Count;

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
                    Categories = result.Data;
                }
            }
            catch(Exception)
            {

            }
        }

        private async Task LoadProductsAsync()
        {
            IsLoading = true;
            try
            {
                var result = await ContentClientService.GetProductsAsync(Filter);

                if (result.Success && result.Data != null)
                {
                    ProductList = result.Data;
                    TotalItems = 25;
                }
            }
            catch (Exception) { }
            finally
            {
                IsLoading = false;
            }
        }

        private async Task ApplyFilterAsync()
        {
            Filter.PageIndex = 1;
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
                Filter.CategoryId = catId;
            }
            else
            {
                Filter.CategoryId = null;
            }
            await ApplyFilterAsync();
        }

        private async Task OnStatusChanged(ChangeEventArgs e)
        {
            if (bool.TryParse(e.Value?.ToString(), out bool status))
            {
                Filter.IsActive = status;
            }
            else
            {
                Filter.IsActive = null;
            }
            await ApplyFilterAsync();
        }

        private async Task ChangePageAsync(int pageIndex)
        {
            if (pageIndex >= 1 && pageIndex <= TotalPages)
            {
                Filter.PageIndex = pageIndex;
                await LoadProductsAsync();
            }
        }

        private async Task OnPageSizeChanged(ChangeEventArgs e)
        {
            if (int.TryParse(e.Value?.ToString(), out int pageSize))
            {
                Filter.PageSize = pageSize;
                Filter.PageIndex = 1;
                await LoadProductsAsync();
            }
        }

        private void ToggleSelectAll(ChangeEventArgs e)
        {
            bool check = (bool)(e.Value ?? false);
            if (check)
            {
                SelectedProductIds = ProductList.Select(p => p.ProductId).ToHashSet();
            }
            else
            {
                SelectedProductIds.Clear();
            }
        }

        private void ToggleSelectProduct(int productId)
        {
            if (SelectedProductIds.Contains(productId))
            {
                SelectedProductIds.Remove(productId);
            }
            else
            {
                SelectedProductIds.Add(productId);
            }
        }

        private async Task ConfirmDelete(ProductListDto product)
        {
            // TODO: Mở Modal Confirm xóa sách
            await Task.CompletedTask;
        }

        private string GetImageUrl(string? url)
        {
            if (string.IsNullOrEmpty(url)) return "https://via.placeholder.com/60x80?text=No+Cover";
            return url.StartsWith("http") ? url : $"https://localhost:7188{url}";
        }

        private int GetDisplayStart() => TotalItems == 0 ? 0 : ((Filter.PageIndex - 1) * Filter.PageSize) + 1;
        private int GetDisplayEnd() => Math.Min(Filter.PageIndex * Filter.PageSize, TotalItems);
    }
}