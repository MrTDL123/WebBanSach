using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using WebApp.Admin.Utilities;
using WebApp.Shared.Dtos.Common;
using WebApp.Shared.Dtos.Management.ContentAndMarketing;

namespace WebApp.Admin.Components.Pages.Content.ProductMetaData
{
    public partial class ProductMetaData
    {
        private string _activeTab = "authors";
        private string _searchTerm = "";
        private string _statusFilter = "";
        private bool _isLoading = false;
        private int _pageIndex = 1;
        private int _pageSize = 10;
        private int TotalPages => (int)Math.Ceiling((double)GetTotalItems() / _pageSize);
        private string? _errorMessage;
        private HashSet<int> _selectedItemIds { get; set; } = new();
        private bool _isAllSelected = false;

        // Danh sách DTO từ Server
        private List<AuthorDto> _authors = new();
        private List<PublisherDto> _publishers = new();
        private List<CategoryDto> _categories = new();
        private List<VoucherDto> _vouchers = new();

        private CategoryModal _categoryModal = default!;
        private AuthorModal _authorModal = default!;
        private PublisherModal _publisherModal = default!;
        private VoucherModal _voucherModal = default!;

        protected override async Task OnInitializedAsync()
        {
            await LoadDataAsync();
        }

        private async Task SwitchTab(string tabName)
        {
            _activeTab = tabName;
            _searchTerm = "";
            _statusFilter = "";
            _selectedItemIds.Clear();
            _isAllSelected = false;
            await LoadDataAsync();
        }

        private async Task LoadDataAsync()
        {
            _isLoading = true;
            _errorMessage = null;
            try
            {
                switch (_activeTab)
                {
                    case "authors":
                        var authorsResult = await ContentClientService.GetAuthorsAsync();
                        if (authorsResult.Success && authorsResult.Data != null)
                            _authors = authorsResult.Data;
                        break;
                    case "publishers":
                        var publishersResult = await ContentClientService.GetPublishersAsync();
                        if (publishersResult.Success && publishersResult.Data != null)
                            _publishers = publishersResult.Data;
                        break;
                    case "categories":
                        var categoriesResult = await ContentClientService.GetCategoriesAsync();
                        if (categoriesResult.Success && categoriesResult.Data != null)
                            _categories = categoriesResult.Data;
                        break;
                    case "vouchers":
                        var vouchersResult = await ContentClientService.GetVouchersAsync();
                        if (vouchersResult.Success && vouchersResult.Data != null)
                            _vouchers = vouchersResult.Data;
                        break;
                }
            }
            catch (Exception ex)
            {
                _errorMessage = "Lỗi hệ thống: Không thể tải dữ liệu: " + ex.Message;
            }
            finally
            {
                _isLoading = false;
            }
        }

        #region --- Filter & Search ---
        // Lấy danh sách đã lọc dựa trên tab hiện tại và từ khóa tìm kiếm
        private List<AuthorDto> GetFilteredAuthors() => _authors
            .Where(
                a => (string.IsNullOrWhiteSpace(_searchTerm) || 
                a.AuthorName.Contains(_searchTerm, StringComparison.OrdinalIgnoreCase)) &&
                (string.IsNullOrEmpty(_statusFilter) || a.IsActive == bool.Parse(_statusFilter))
            )   
            .ToList();

        private List<PublisherDto> GetFilteredPublishers() => _publishers
            .Where(
                p => (string.IsNullOrWhiteSpace(_searchTerm) || 
                p.PublisherName.Contains(_searchTerm, StringComparison.OrdinalIgnoreCase)) &&
                (string.IsNullOrEmpty(_statusFilter) || p.IsActive == bool.Parse(_statusFilter))
            )
            .ToList();

        private List<CategoryDto> GetFilteredCategories() => _categories
            .Where(
                c => (string.IsNullOrWhiteSpace(_searchTerm) || 
                    c.CategoryName.Contains(_searchTerm, StringComparison.OrdinalIgnoreCase)) &&
                        (string.IsNullOrEmpty(_statusFilter) || 
                        c.IsActive == bool.Parse(_statusFilter)
                )
            )
            .ToList();

        private List<VoucherDto> GetFilteredVouchers() => _vouchers
            .Where(
                v => (string.IsNullOrWhiteSpace(_searchTerm) || 
                    v.Code.Contains(_searchTerm, StringComparison.OrdinalIgnoreCase)) &&
                        (string.IsNullOrEmpty(_statusFilter) || 
                        v.IsActive == bool.Parse(_statusFilter)
                )
            )
            .ToList();

        private List<AuthorDto> _pageAuthors => GetFilteredAuthors().Skip((_pageIndex - 1)* _pageSize).Take(_pageSize).ToList();
        private List<PublisherDto> _pagePublishers => GetFilteredPublishers().Skip((_pageIndex - 1) * _pageSize).Take(_pageSize).ToList();
        private List<CategoryDto> _pageCategories => GetFilteredCategories().Skip((_pageIndex - 1) * _pageSize).Take(_pageSize).ToList();
        private List<VoucherDto> _pageVouchers => GetFilteredVouchers().Skip((_pageIndex - 1) * _pageSize).Take(_pageSize).ToList();

        private int GetTotalItems() => _activeTab switch
        {
            "authors" => GetFilteredAuthors().Count,
            "publishers" => GetFilteredPublishers().Count,
            "categories" => GetFilteredCategories().Count,
            "vouchers" => GetFilteredVouchers().Count,
            _ => 0
        };

        private void OnSearchInput(ChangeEventArgs e)
        {
            _searchTerm = e.Value?.ToString() ?? "";
            _pageIndex = 1; // Reset về trang đầu tiên khi tìm kiếm
        }

        private void OnStatusFilterChanged(ChangeEventArgs e)
        {
            _statusFilter = e.Value?.ToString() ?? "";
            _pageIndex = 1; // Reset về trang đầu tiên khi thay đổi trạng thái
        }

        private void ChangePage(int pageIndex)
        {
            if (pageIndex >= 1 && pageIndex <= TotalPages)
            {
                _pageIndex = pageIndex;
            }
        }

        private void OnPageSizeChanged(ChangeEventArgs e)
        {
            if (int.TryParse(e.Value?.ToString(), out int pageSize))
            {
                _pageSize = pageSize;
                _pageIndex = 1; // Reset về trang đầu tiên khi thay đổi kích thước trang
            }
        }

        private int GetDisplayStart() => GetTotalItems() == 0 ? 0 : ((_pageIndex - 1) * _pageSize) + 1;
        private int GetDisplayEnd() => Math.Min(_pageIndex * _pageSize, GetTotalItems());

        #endregion

        #region --- QUẢN LÝ MODAL (MỞ MODAL THÊM / SỬA) ---

        // Mở Modal Thêm mới
        private void OpenCreateModal()
        {
            switch (_activeTab)
            {
                case "authors":
                    _authorModal.OpenForCreate();
                    break;
                case "categories":
                    _categoryModal.OpenForCreate();
                    break;
                case "publishers":
                    _publisherModal.OpenForCreate();
                    break;
                case "vouchers":
                    _voucherModal.OpenForCreate();
                    break;
            }
        }

        // Mở Modal Sửa
        private void OpenEditAuthorModal(AuthorDto dto) => _authorModal.OpenForEdit(dto);
        private void OpenEditCategoryModal(CategoryDto dto) => _categoryModal.OpenForEdit(dto);
        private void OpenEditPublisherModal(PublisherDto dto) => _publisherModal.OpenForEdit(dto);
        private void OpenEditVoucherModal(VoucherDto dto) => _voucherModal.OpenForEdit(dto);
        #endregion

        private async Task DeleteAuthorAsync(int id)
        {
            try
            {
                var result = await ContentClientService.DeleteAuthorAsync(id);
                if (!result.Success)
                {
                    _errorMessage = result.Message ?? "Xóa tác giả thất bại";
                    Toast.ShowError(_errorMessage);
                    return;
                }
                await LoadDataAsync();
                Toast.ShowSuccess("Xóa tác giả thành công");
            }
            catch (Exception ex)
            {
                _errorMessage = "Lỗi hệ thống: " + ex.Message;
            }
        }

        private async Task DeletePublisherAsync(int id)
        {
            try
            {
                var result = await ContentClientService.DeletePublisherAsync(id);
                if (!result.Success)
                {
                    _errorMessage = result.Message ?? "Xóa nhà xuất bản thất bại";
                    Toast.ShowError(_errorMessage);
                    return;
                }
                Toast.ShowSuccess("Xóa nhà xuất bản thành công");
                await LoadDataAsync();
            }
            catch (Exception ex) 
            {
                _errorMessage = "Lỗi hệ thống: " + ex.Message;
            }
        }

        private async Task DeleteCategoryAsync(int id)
        {
            try
            {
                var result = await ContentClientService.DeleteCategoryAsync(id);
                if (!result.Success)
                {
                    _errorMessage = result.Message ?? "Xóa danh mục thất bại";
                    Toast.ShowError(_errorMessage);
                    return;
                }
                Toast.ShowSuccess("Xóa danh mục thành công");
                await LoadDataAsync();
            }
            catch (Exception ex) 
            {
                _errorMessage = "Lỗi hệ thống: " + ex.Message;
            }
        }

        private async Task DeleteVoucherAsync(int id)
        {
            try
            {
                var result = await ContentClientService.DeleteVoucherAsync(id);
                if (!result.Success)
                {
                    _errorMessage = result.Message ?? "Xóa mã giảm giá thất bại";
                    Toast.ShowError(_errorMessage);
                    return;
                }
                Toast.ShowSuccess("Xóa mã giảm giá thành công");
                await LoadDataAsync();
            }
            catch (Exception ex) 
            {
                _errorMessage = "Lỗi hệ thống: " + ex.Message;
            }
        }

        private string GetActiveTabName() => _activeTab switch
        {
            "authors" => "tác giả",
            "publishers" => "nhà xuất bản",
            "categories" => "danh mục",
            "vouchers" => "mã giảm giá",
            _ => ""
        };

        private string GetSearchPlaceholder() => _activeTab switch
        {
            "authors" => "Nhập tên tác giả...",
            "publishers" => "Nhập tên nhà xuất bản...",
            "categories" => "Nhập tên danh mục...",
            "vouchers" => "Nhập mã voucher...",
            _ => "Tìm kiếm..."
        };
        public List<int> GetCurrentPageItemIds() => _activeTab switch
        {
            "authors" => _pageAuthors.Select(a => a.AuthorId).ToList(),
            "publishers" => _pagePublishers.Select(p => p.PublisherId).ToList(),
            "categories" => _pageCategories.Select(c => c.CategoryId).ToList(),
            "vouchers" => _pageVouchers.Select(v => v.VoucherId).ToList(),
            _ => new()
        };

        private void ToggleSelectAll(ChangeEventArgs e)
        {
            bool check = (bool)(e.Value ?? false);
            _isAllSelected = check;
            var currentPageItemIds = GetCurrentPageItemIds();

            if (check)
            {
                foreach (var itemId in currentPageItemIds)
                {
                    _selectedItemIds.Add(itemId);
                }
            }
            else
            {
                foreach (var itemId in currentPageItemIds)
                {
                    _selectedItemIds.Remove(itemId);
                }
            }
        }

        private void ToggleSelectItem(int itemId)
        {
            if (_selectedItemIds.Contains(itemId))
            {
                _selectedItemIds.Remove(itemId);
            }
            else
            {
                _selectedItemIds.Add(itemId);
            }
            CheckIfAllSelected();
        }

        private void CheckIfAllSelected()
        {
            var currentPageItemIds = GetCurrentPageItemIds();
            _isAllSelected = currentPageItemIds.All(id => _selectedItemIds.Contains(id));
        }
    }
}
