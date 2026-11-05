using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using WebApp.Shared.Dtos.Management.ContentAndMarketing;
using WebApp.Admin.Utilities;

namespace WebApp.Admin.Components.Pages.Content.Products
{
    public partial class EditProduct
    {
        [Parameter] 
        public int Id { get; set; }

        private UpdateProductDto _updateProductModel { get; set; } = new();
        private List<CategoryDto> _categories { get; set; } = new();
        private List<AuthorDto> _authors { get; set; } = new();
        private List<PublisherDto> _publishers { get; set; } = new();
        private List<ProductGalleryItemDto> _galleryImages { get; set; } = new();

        private string? _previewMainImage  { get; set; }
        private string? _errorMessage { get; set; }
        private bool _isLoading { get; set; } = true;
        private bool _isSubmitting { get; set; }

        protected override async Task OnInitializedAsync()
        {
            await LoadDropdownDataAsync();
            await LoadProductDetailAsync();
        }

        private async Task LoadDropdownDataAsync()
        {
            try
            {
                var resultCategories = ContentClientService.GetCategoriesAsync();
                var resultAuthors = ContentClientService.GetAuthorsAsync();
                var resultPublishers = ContentClientService.GetPublishersAsync();

                await Task.WhenAll(resultCategories, resultAuthors, resultPublishers);

                if (resultCategories.Result.Success && resultCategories.Result.Data != null) _categories = resultCategories.Result.Data;
                if (resultAuthors.Result.Success && resultAuthors.Result.Data != null) _authors = resultAuthors.Result.Data;
                if (resultPublishers.Result.Success && resultPublishers.Result.Data != null) _publishers = resultPublishers.Result.Data;
            }
            catch (Exception ex)
            {
                _errorMessage = "Không thể tải danh sách dữ liệu phụ: " + ex.Message;
            }
        }

        private async Task LoadProductDetailAsync()
        {
            _isLoading = true;
            try
            {
                var result = await ContentClientService.GetProductForUpdateAsync(Id);
                if (result.Success && result.Data != null)
                {
                    _updateProductModel = result.Data;

                    if (!string.IsNullOrEmpty(_updateProductModel.MainImageUrl))
                    {
                        _previewMainImage  = _updateProductModel.MainImageUrl.StartsWith("http") ? _updateProductModel.MainImageUrl : $"https://localhost:7188{_updateProductModel.MainImageUrl}";
                    }

                    // Tải ảnh phụ cũ từ DB vào danh sách Gallery để hiển thị
                    if (_updateProductModel.GalleryImages != null)
                    {
                        _galleryImages = _updateProductModel.GalleryImages;
                    }
                }
                else
                {
                    _errorMessage = "Không tìm thấy thông tin sản phẩm.";
                }
            }
            catch (Exception ex)
            {
                _errorMessage = "Lỗi khi tải chi tiết sách: " + ex.Message;
            }
            finally
            {
                _isLoading = false;
            }
        }

        private async Task HandleMainImageSelected(InputFileChangeEventArgs e)
        {
            var file = e.File;
            if (file != null)
            {
                _updateProductModel.MainImageBase64 = await ImageHelper.ConvertToBase64Async(file);
                _updateProductModel.MainImageFileName = file.Name;
                _previewMainImage  = _updateProductModel.MainImageBase64;
            }
        }

        private void RemoveMainImage()
        {
            _updateProductModel.MainImageBase64 = null;
            _updateProductModel.MainImageFileName = null;
            _previewMainImage  = null;
        }

        private async Task HandleSubmitAsync()
        {
            _isSubmitting = true;
            _errorMessage = null;

            try
            {
                _updateProductModel.GalleryImages = _galleryImages;
                var result = await ContentClientService.UpdateProductAsync(_updateProductModel);
                if (result.Success)
                {
                    Navigation.NavigateTo("/management/content/products");
                }
                else
                {
                    _errorMessage = result.Message ?? "Cập nhật sản phẩm thất bại.";
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
            if(e.FileCount > 0)
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

        // Hàm lấy đúng đường dẫn ảnh (ảnh cũ dùng ImageUrl, ảnh mới dùng Base64Data)
        private string GetGalleryImageSrc(ProductGalleryItemDto item)
        {
            if (!string.IsNullOrEmpty(item.Base64Data))
                return item.Base64Data;

            if (!string.IsNullOrEmpty(item.ImageUrl))
                return item.ImageUrl.StartsWith("http")
                    ? item.ImageUrl
                    : $"https://localhost:7188{item.ImageUrl}";
            return string.Empty;
        }

        // Xóa ảnh phụ theo vị trí index trong danh sách
        private void RemoveGalleryImage(ProductGalleryItemDto item)
        {
            if (item.ImageId > 0)
            {
                item.IsDeleted = true;
            }
            else
            {
                _galleryImages.Remove(item);
            }

            int order = 1;

            foreach (var image in _galleryImages.Where(x => !x.IsDeleted))
            {
                image.DisplayOrder = order++;
            }
        }
    }
}
