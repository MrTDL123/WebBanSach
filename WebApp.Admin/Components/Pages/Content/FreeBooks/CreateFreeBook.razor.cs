using Microsoft.AspNetCore.Components.Forms;
using WebApp.Shared.Dtos.Management.ContentAndMarketing;
using WebApp.Admin.Utilities;

namespace WebApp.Admin.Components.Pages.Content.FreeBooks
{
    public partial class CreateFreeBook
    {
        private CreateFreeBookDto _createModel { get; set; } = new();
        private string? _previewCoverImage { get; set; }
        private string? _errorMessage { get; set; }
        private bool _isSubmitting { get; set; }

        private async Task HandleCoverImageSelected(InputFileChangeEventArgs e)
        {
            var file = e.File;
            if (file != null)
            {
                try
                {
                    _createModel.CoverImageBase64 = await ImageHelper.ConvertToBase64Async(file);
                    _createModel.CoverImageFileName = file.Name;
                    _previewCoverImage = _createModel.CoverImageBase64;
                }
                catch (Exception ex)
                {
                    _errorMessage = "Lỗi xử lý ảnh: " + ex.Message;
                }
            }
        }

        private void RemoveCoverImage()
        {
            _createModel.CoverImageBase64 = null;
            _createModel.CoverImageFileName = null;
            _previewCoverImage = null;
        }

        private async Task HandleSubmitAsync()
        {
            _isSubmitting = true;
            _errorMessage = null;

            try
            {
                var result = await ContentClientService.CreateFreeBookAsync(_createModel);
                if (result.Success)
                {
                    Navigation.NavigateTo("/management/content/freebooks");
                }
                else
                {
                    _errorMessage = result.Message ?? "Thêm sách mới thất bại.";
                }
            }
            catch (Exception ex)
            {
                _errorMessage = "Lỗi hệ thống: " + ex.Message;
            }
            finally
            {
                _isSubmitting = false;
            }
        }
    }
}