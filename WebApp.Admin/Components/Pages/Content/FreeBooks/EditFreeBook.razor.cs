using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using WebApp.Shared.Dtos.Management.ContentAndMarketing;
using WebApp.Admin.Utilities;

namespace WebApp.Admin.Components.Pages.Content.FreeBooks
{
    public partial class EditFreeBook
    {
        [Parameter]
        public int Id { get; set; }

        private UpdateFreeBookDto _updateModel { get; set; } = new();
        private string? _previewCoverImage { get; set; }
        private string? _errorMessage { get; set; }
        private bool _isLoading { get; set; } = true;
        private bool _isSubmitting { get; set; }

        protected override async Task OnInitializedAsync()
        {
            await LoadFreeBookForUpdateAsync();
        }

        private async Task LoadFreeBookForUpdateAsync()
        {
            _isLoading = true;
            try
            {
                var result = await ContentClientService.GetFreeBookForUpdateAsync(Id);
                if (result.Success && result.Data != null)
                {
                    _updateModel = result.Data;

                    if (!string.IsNullOrEmpty(_updateModel.CoverImageUrl))
                    {
                        _previewCoverImage = _updateModel.CoverImageUrl.StartsWith("http")
                            ? _updateModel.CoverImageUrl
                            : $"https://localhost:7188{_updateModel.CoverImageUrl}";
                    }
                }
                else
                {
                    _errorMessage = "Không tìm thấy thông tin sách.";
                }
            }
            catch (Exception ex)
            {
                _errorMessage = "Lỗi khi tải dữ liệu sách: " + ex.Message;
            }
            finally
            {
                _isLoading = false;
            }
        }

        private async Task HandleCoverImageSelected(InputFileChangeEventArgs e)
        {
            var file = e.File;
            if (file != null)
            {
                _updateModel.CoverImageBase64 = await ImageHelper.ConvertToBase64Async(file);
                _updateModel.CoverImageFileName = file.Name;
                _previewCoverImage = _updateModel.CoverImageBase64;
            }
        }

        private void RemoveCoverImage()
        {
            _updateModel.CoverImageBase64 = null;
            _updateModel.CoverImageFileName = null;
            _previewCoverImage = null;
        }

        private async Task HandleSubmitAsync()
        {
            _isSubmitting = true;
            _errorMessage = null;

            try
            {
                var result = await ContentClientService.UpdateFreeBookAsync(_updateModel);
                if (result.Success)
                {
                    Navigation.NavigateTo("/management/content/freebooks");
                }
                else
                {
                    _errorMessage = result.Message ?? "Cập nhật sách thất bại.";
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