using Microsoft.AspNetCore.Components;
using WebApp.Shared.Dtos.Management.ContentAndMarketing;

namespace WebApp.Admin.Components.Pages.Content.ProductMetaData
{
    public partial class AuthorModal
    {
        private bool _isOpen = false;
        private bool _isEdit = false;
        private bool _isSubmitting = false;
        private string? _errorMessage;

        private UpdateAuthorDto _updateAuthorModel = new();
        private CreateAuthorDto _createAuthorModel = new();

        // Sự kiện báo cho trang cha biết để reload lại bảng sau khi Lưu thành công
        [Parameter]
        public EventCallback OnSuccess { get; set; }

        public void OpenForCreate()
        {
            _isEdit = false;
            _createAuthorModel = new CreateAuthorDto(); // Reset form rỗng
            _errorMessage = null;
            _isOpen = true;
            StateHasChanged();
        }

        public void OpenForEdit(AuthorDto dto)
        {
            _isEdit = true;
            _updateAuthorModel = new UpdateAuthorDto
            {
                AuthorId = dto.AuthorId,
                AuthorName = dto.AuthorName,
                Biography = dto.Biography,
                IsActive = dto.IsActive
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
                    var result = await ContentClientService.UpdateAuthorAsync(_updateAuthorModel);
                    if (!result.Success)
                    {
                        _errorMessage = result.Message ?? "Cập nhật tác giả thất bại";
                        Toast.ShowError(_errorMessage);
                        return;
                    }
                }
                else
                {
                    var result = await ContentClientService.CreateAuthorAsync(_createAuthorModel);
                    if (!result.Success)
                    {
                        _errorMessage = result.Message ?? "Thêm mới tác giả thất bại";
                        Toast.ShowError(_errorMessage);
                        return;
                    }
                }
                Close();
                Toast.ShowSuccess(_isEdit ? "Cập nhật tác giả thành công" : "Thêm mới tác giả thành công");
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
