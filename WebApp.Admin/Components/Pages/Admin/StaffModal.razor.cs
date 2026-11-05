using Microsoft.AspNetCore.Components;
using WebApp.Shared.Dtos.Management.Admin;

namespace WebApp.Admin.Components.Pages.Admin
{
    public partial class StaffModal
    {
        private bool _isOpen = false;
        private bool _isEdit = false;
        private bool _isSubmitting = false;
        private string? _errorMessage;
        private string _staffName = string.Empty;

        private CreateEmployeeDto _createStaffModel = new();
        private UpdateEmployeeDto _updateRoleModel = new();

        [Parameter]
        public EventCallback OnSuccess { get; set; }

        public void OpenForCreate()
        {
            _isEdit = false;
            _createStaffModel = new CreateEmployeeDto();
            _errorMessage = null;
            _isOpen = true;
            StateHasChanged();
        }

        public void OpenForEdit(UserListItemDto dto)
        {
            _isEdit = true;
            _staffName = dto.FullName;
            _updateRoleModel = new UpdateEmployeeDto
            {
                UserId = dto.UserId,
                Role = dto.Roles.FirstOrDefault(r => r != "Customer") ?? string.Empty
            };
            _errorMessage = null;
            _isOpen = true;
            StateHasChanged();
        }

        public void Close()
        {
            _isOpen = false;
            _errorMessage = null;
        }

        private async Task HandleSubmitAsync()
        {
            _isSubmitting = true;
            _errorMessage = null;

            try
            {
                if (_isEdit)
                {
                    var result = await AdminClientService.UpdateStaffRoleAsync(_updateRoleModel);
                    if (!result.Success)
                    {
                        _errorMessage = result.Message ?? "Cập nhật quyền nhân viên thất bại";
                        Toast.ShowError(_errorMessage);
                        return;
                    }
                    Toast.ShowSuccess("Cập nhật quyền nhân viên thành công");
                }
                else
                {
                    var result = await AdminClientService.CreateStaffAsync(_createStaffModel);
                    if (!result.Success)
                    {
                        _errorMessage = result.Message ?? "Thêm mới nhân viên thất bại";
                        Toast.ShowError(_errorMessage);
                        return;
                    }
                    Toast.ShowSuccess("Thêm mới nhân viên thành công");
                }

                Close();
                await OnSuccess.InvokeAsync();
            }
            catch (Exception ex)
            {
                _errorMessage = $"Đã xảy ra lỗi: {ex.Message}";
            }
            finally
            {
                _isSubmitting = false;
            }
        }
    }
}