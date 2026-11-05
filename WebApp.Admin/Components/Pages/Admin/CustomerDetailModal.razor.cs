using WebApp.Shared.Dtos.Management.Admin;

namespace WebApp.Admin.Components.Pages.Admin
{
    public partial class CustomerDetailModal
    {
        private bool _isOpen = false;
        private bool _isLoading = false;
        private string? _errorMessage;
        private UserListItemDto? _customer;

        public async Task OpenForDetail(string userId)
        {
            _isOpen = true;
            _isLoading = true;
            _errorMessage = null;
            StateHasChanged();

            try
            {
                var result = await AdminClientService.GetCustomerDetailAsync(userId);
                if (result.Success && result != null)
                {
                    _customer = result.Data;
                }
            }
            catch (Exception ex)
            {
                _customer = null;
                _errorMessage = $"Đã xảy ra lỗi: {ex.Message}";
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
            _customer = null;
            _errorMessage = null;
        }
    }
}
