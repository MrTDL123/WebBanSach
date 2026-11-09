using Microsoft.AspNetCore.Components;
using WebApp.Shared.Dtos.Management.ContentAndMarketing;

namespace WebApp.Admin.Components.Pages.Content.ProductMetaData
{
    public partial class CategoryModal
    {
        private bool _isOpen = false;
        private bool _isEdit = false;
        private bool _isSubmitting = false;
        private string? _errorMessage;

        private UpdateCategoryDto _updateCategoryModel = new();
        private CreateCategoryDto _createCategoryModel = new();

        // Sự kiện báo cho trang cha biết để reload lại bảng sau khi Lưu thành công
        [Parameter] 
        public EventCallback OnSuccess { get; set; }

        public void OpenForCreate()
        {
            _isEdit = false;
            _createCategoryModel = new CreateCategoryDto(); // Reset form rỗng
            _errorMessage = null;
            _isOpen = true;
            StateHasChanged();
        }

        public void OpenForEdit(CategoryDto dto)
        {
            _isEdit = true;
            _updateCategoryModel = new UpdateCategoryDto
            {
                CategoryId = dto.CategoryId,
                CategoryName = dto.CategoryName,
                Description = dto.Description,
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
                    var result = await ContentClientService.UpdateCategoryAsync(_updateCategoryModel);
                    if (!result.Success)
                    {
                        _errorMessage = result.Message ?? "Cập nhật danh mục thất bại";
                        Toast.ShowError(_errorMessage);
                        return;
                    }
                }
                else
                {
                    var result = await ContentClientService.CreateCategoryAsync(_createCategoryModel);
                    if (!result.Success)
                    {
                        _errorMessage = result.Message ?? "Thêm mới danh mục thất bại";
                        Toast.ShowError(_errorMessage);
                        return;
                    }
                }
                Close();
                Toast.ShowSuccess(_isEdit ? "Cập nhật danh mục thành công" : "Thêm mới danh mục thành công");
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
