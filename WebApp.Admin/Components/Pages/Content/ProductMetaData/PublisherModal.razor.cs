using Microsoft.AspNetCore.Components;
using WebApp.Shared.Dtos.Management.ContentAndMarketing;

namespace WebApp.Admin.Components.Pages.Content.ProductMetaData
{
    public partial class PublisherModal
    {
        private bool _isOpen = false;
        private bool _isEdit = false;
        private bool _isSubmitting = false;
        private string? _errorMessage;

        private UpdatePublisherDto _updatePublisherModel = new();
        private CreatePublisherDto _createPublisherModel = new();

        // Sự kiện báo cho trang cha biết để reload lại bảng sau khi Lưu thành công
        [Parameter]
        public EventCallback OnSuccess { get; set; }

        public void OpenForCreate()
        {
            _isEdit = false;
            _createPublisherModel = new CreatePublisherDto(); // Reset form rỗng
            _errorMessage = null;
            _isOpen = true;
            StateHasChanged();
        }

        public void OpenForEdit(PublisherDto dto)
        {
            _isEdit = true;
            _updatePublisherModel = new UpdatePublisherDto
            {
                PublisherId = dto.PublisherId,
                PublisherName = dto.PublisherName,
                Address = dto.Address,
                PhoneNumber = dto.PhoneNumber,
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
                    var result = await ContentClientService.UpdatePublisherAsync(_updatePublisherModel);
                    if (!result.Success)
                    {
                        _errorMessage = result.Message ?? "Cập nhật nhà xuất bản thất bại";
                        Toast.ShowError(_errorMessage);
                        return;
                    }
                }
                else
                {
                    var result = await ContentClientService.CreatePublisherAsync(_createPublisherModel);
                    if (!result.Success)
                    {
                        _errorMessage = result.Message ?? "Thêm mới nhà xuất bản thất bại";
                        Toast.ShowError(_errorMessage);
                        return;
                    }
                }
                Close();
                Toast.ShowSuccess(_isEdit ? "Cập nhật nhà xuất bản thành công" : "Thêm mới nhà xuất bản thành công");
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
