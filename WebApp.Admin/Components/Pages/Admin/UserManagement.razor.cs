using Microsoft.AspNetCore.Components;
using WebApp.Shared.Dtos.Management.Admin;

namespace WebApp.Admin.Components.Pages.Admin
{
    public partial class UserManagement
    {
        private string _activeTab = "customers";
        private string _searchTerm = "";
        private string _statusFilter = "";
        private string _roleFilter = "";
        private bool _isLoading = false;

        private int _pageIndex = 1;
        private int _pageSize = 10;
        private int _totalCustomers = 0;
        private int _totalPages => (int)Math.Ceiling((double)_totalCustomers / _pageSize);
        private string? _errorMessage { get; set; }

        private List<UserListItemDto> _customers = new();
        private List<UserListItemDto> _allStaff = new();
        private List<UserListItemDto> _displayedStaff = new();

        private HashSet<string> _selectedItemIds { get; set; } = new();
        private bool _isAllSelected = false;

        private StaffModal? _staffModal;
        private BanUserModal? _banUserModal;
        private CustomerDetailModal? _customerDetailModal;

        protected override async Task OnInitializedAsync()
        {
            await LoadDataAsync();
        }

        private async Task SwitchTab(string tab)
        {
            if (_activeTab == tab) return;

            _activeTab = tab;
            _searchTerm = "";
            _statusFilter = "";
            _roleFilter = "";
            _pageIndex = 1;
            _selectedItemIds.Clear();
            _isAllSelected = false;

            await LoadDataAsync(forceReload: true);
        }

        private async Task RefreshDataAsync()
        {
            await LoadDataAsync(forceReload: true);
        }

        private async Task LoadDataAsync(bool forceReload = false)
        {
            _isLoading = true;
            StateHasChanged();

            try
            {
                if (_activeTab == "customers")
                {
                    var filter = new UserFilterDto
                    {
                        SearchTerm = _searchTerm,
                        IsActive = string.IsNullOrEmpty(_statusFilter) ? null : bool.Parse(_statusFilter),
                        PageIndex = _pageIndex,
                        PageSize = _pageSize
                    };

                    var result = await AdminClientService.GetCustomersPagedAsync(filter);
                    if (result.Success && result.Data != null)
                    {
                        _customers = result.Data.Items;
                        _totalCustomers = result.Data.TotalCount;
                    }
                }
                else if (_activeTab == "staff")
                {
                    if (!_allStaff.Any() || forceReload)
                    {
                        var result = await AdminClientService.GetAllStaffAsync();
                        if (result.Success && result.Data != null)
                        {
                            _allStaff = result.Data;
                        }
                    }

                    ApplyClientSideStaffFilter();
                }
            }
            catch (Exception ex)
            {
                _errorMessage = $"Lỗi khi tải dữ liệu: {ex.Message}";
            }
            finally
            {
                _isLoading = false;
                StateHasChanged();
            }
        }

        private void ApplyClientSideStaffFilter()
        {
            var query = _allStaff.AsEnumerable();

            if (!string.IsNullOrWhiteSpace(_searchTerm))
            {
                var term = _searchTerm.Trim().ToLower();
                query = query.Where(s =>
                    s.FullName.Contains(term, StringComparison.CurrentCultureIgnoreCase) ||
                    (s.Email != null && s.Email.Contains(term, StringComparison.CurrentCultureIgnoreCase)) ||
                    (s.PhoneNumber != null && s.PhoneNumber.Contains(term, StringComparison.CurrentCultureIgnoreCase)));
            }

            if (!string.IsNullOrEmpty(_roleFilter))
            {
                query = query.Where(s => s.Roles.Contains(_roleFilter));
            }

            if (!string.IsNullOrEmpty(_statusFilter))
            {
                bool isActive = bool.Parse(_statusFilter);
                query = query.Where(s => s.IsActive == isActive);
            }

            _displayedStaff = query.ToList();
        }

        private async Task OnSearchInput(ChangeEventArgs e)
        {
            _searchTerm = e.Value?.ToString() ?? "";

            if (_activeTab == "staff")
            {
                ApplyClientSideStaffFilter();
            }
            else
            {
                _pageIndex = 1;
                await LoadDataAsync();
            }
        }

        private async Task OnStatusFilterChanged(ChangeEventArgs e)
        {
            _statusFilter = e.Value?.ToString() ?? "";
            if (_activeTab == "staff")
            {
                ApplyClientSideStaffFilter();
            }
            else
            {
                _pageIndex = 1;
                await LoadDataAsync();
            }
        }

        private async Task ChangePage(int newPage)
        {
            if (newPage >= 1 && newPage <= _totalPages)
            {
                _pageIndex = newPage;
                await LoadDataAsync();
            }
        }

        private async Task OnPageSizeChanged(ChangeEventArgs e)
        {
            if (int.TryParse(e.Value?.ToString(), out int size))
            {
                _pageSize = size;
                _pageIndex = 1;
                await LoadDataAsync();
            }
        }

        private string GetSearchPlaceholder() => _activeTab == "customers"
            ? "Tìm kiếm khách hàng theo tên, email, sđt..."
            : "Tìm kiếm nhân viên theo tên, email, sđt...";

        private int GetDisplayStart() => _totalCustomers == 0 ? 0 : ((_pageIndex - 1) * _pageSize) + 1;

        private int GetDisplayEnd() => Math.Min(_pageIndex * _pageSize, _totalCustomers);

        private string GetRoleDisplayName(List<string> roles)
        {
            var primaryRole = roles.FirstOrDefault(r => r != "Customer") ?? "N/A";
            return primaryRole switch
            {
                "ContentManager" => "Quản lý Nội dung",
                "OrderManager" => "Quản lý Đơn hàng",
                "CustomerSupport" => "CSKH",
                "WarehouseManager" => "Quản lý Kho",
                "Accountant" => "Kế toán",
                _ => primaryRole
            };
        }
        private string GetAvatarUrl(UserListItemDto user)
        {
            if (!string.IsNullOrEmpty(user.AvatarUrl))
            {
                return user.AvatarUrl.StartsWith("http") ? user.AvatarUrl : $"https://localhost:7188{user.AvatarUrl}";
            }

            return $"https://ui-avatars.com/api/?name={Uri.EscapeDataString(user.FullName)}&background=E31837&color=fff";
        }

        private void ToggleSelectAll(ChangeEventArgs e)
        {
            _isAllSelected = (bool)(e.Value ?? false);
            _selectedItemIds.Clear();

            if (_isAllSelected)
            {
                var items = _activeTab == "customers"
                    ? _customers.Select(c => c.UserId)
                    : _displayedStaff.Select(s => s.UserId);

                foreach (var id in items) _selectedItemIds.Add(id);
            }
        }

        private void ToggleSelectItem(string userId)
        {
            if (!_selectedItemIds.Remove(userId))
            {
                _selectedItemIds.Add(userId);
            }
        }

        private void OpenCreateStaffModal() => _staffModal?.OpenForCreate();
        private void OpenEditRoleModal(UserListItemDto staff) => _staffModal?.OpenForEdit(staff);
        private void OpenCustomerDetailModal(UserListItemDto customer) => _customerDetailModal?.OpenForDetail(customer.UserId);
        private void OpenBanModal(string userId, string name, bool isStaff) => _banUserModal?.OpenForBan(userId, name, isStaff);

        private async Task UnbanUserAsync(string userId)
        {
            try
            {
                var result = await AdminClientService.UnbanUserAsync(userId);
                if (!result.Success)
                {
                    _errorMessage = result.Message;
                }
                Toast.ShowSuccess("Đã mở khóa tài khoản thành công!");
                await LoadDataAsync(forceReload: true);
            }
            catch (Exception ex)
            {
                _errorMessage = $"Đã xảy ra lỗi: {ex.Message}";
            }
        }
    }
}
