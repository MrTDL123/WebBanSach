using Microsoft.AspNetCore.Components.Forms;
using WebApp.Shared.Dtos.Management.ContentAndMarketing;
using WebApp.Admin.Utilities;

namespace WebApp.Admin.Components.Pages.Content.Products
{
    public partial class CreateProduct
    {
        private CreateProductDto _createProductModel { get; set; } = new();
        private List<CategoryDto> _categories { get; set; } = new();
        private List<AuthorDto> _authors { get; set; } = new();
        private List<PublisherDto> _publishers { get; set; } = new();
        private List<ProductGalleryItemDto> _galleryImages { get; set; } = new();

        private string? _previewMainImage { get; set; }
        private string? _errorMessage { get; set; }
        private bool _isSubmitting { get; set; }

        protected override async Task OnInitializedAsync()
        {
            await LoadDropdownDataAsync();
        }

        private async Task LoadDropdownDataAsync()
        {
            try
            {
                var resultCategories = ContentClientService.GetCategoriesAsync();
                var resultAuthors = ContentClientService.GetAuthorsAsync();
                var resultPublihsers = ContentClientService.GetPublishersAsync();

                await Task.WhenAll(resultCategories, resultAuthors, resultPublihsers);

                if (resultCategories.Result.Success && resultCategories.Result.Data != null) _categories = resultCategories.Result.Data;
                if (resultAuthors.Result.Success && resultAuthors.Result.Data != null) _authors = resultAuthors.Result.Data;
                if (resultPublihsers.Result.Success && resultPublihsers.Result.Data != null) _publishers = resultPublihsers.Result.Data;
            }
            catch (Exception ex)
            {
                _errorMessage = "Không thể tải danh sách dữ liệu phụ: " + ex.Message;
            }
        }

        private async Task HandleMainImageSelected(InputFileChangeEventArgs e)
        {
            var file = e.File;
            if (file != null)
            {
                try
                {
                    _createProductModel.MainImageBase64 = await ImageHelper.ConvertToBase64Async(file);
                    _createProductModel.MainImageFileName = file.Name;
                    _previewMainImage = _createProductModel.MainImageBase64;
                }
                catch (Exception ex)
                {
                    _errorMessage = "Lỗi xử lý ảnh: " + ex.Message;
                }

            }            
        }

        private void RemoveMainImage()
        {
            _createProductModel.MainImageBase64 = null;
            _createProductModel.MainImageFileName = null;
            _previewMainImage = null;
        }

        private async Task HandleSubmitAsync()
        {
            _isSubmitting = true;
            _errorMessage = null;

            try
            {
                _createProductModel.GalleryImages = _galleryImages;
                var result = await ContentClientService.CreateProductAsync(_createProductModel);
                if (result.Success)
                {
                    Navigation.NavigateTo("/management/content/products");
                }
                else
                {
                    _errorMessage = result.Message ?? "Tạo sách mới thất bại.";
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

        // Xử lý chọn nhiều ảnh phụ cùng lúc (có thể chọn thêm nhiều lần)
        private async Task HandleGalleryImagesSelected(InputFileChangeEventArgs e)
        {
            if (e.FileCount > 0) 
            {
                foreach (var file in e.GetMultipleFiles(maximumFileCount: 20))
                {
                    _galleryImages.Add(new ProductGalleryItemDto
                    {
                        ImageId = 0,
                        Base64Data = await ImageHelper.ConvertToBase64Async(file),
                        FileName = file.Name,
                        DisplayOrder = _galleryImages.Count + 1
                    });
                }
            }
        }

        // Xóa ảnh phụ theo vị trí index trong danh sách
        private void RemoveGalleryImage(int index)
        {
            if (index >= 0 && index < _galleryImages.Count)
            {
                _galleryImages.RemoveAt(index);
                // Cập nhật lại DisplayOrder cho đúng thứ tự
                for (int i = 0; i < _galleryImages.Count; i++)
                {
                    _galleryImages[i].DisplayOrder = i + 1;
                }
            }
        }
    }
}
