using Microsoft.AspNetCore.Components;
using WebApp.Shared.Dtos.Common;
using WebApp.Shared.Dtos.Management.ContentAndMarketing;

namespace WebApp.Admin.Components.Pages.Content.ProductMetaData
{
    public partial class VoucherModal
    {
        private bool _isOpen = false;
        private bool _isEdit = false;
        private bool _isSubmitting = false;
        private string? _errorMessage;

        private UpdateVoucherDto _updateVoucherModel = new();
        private CreateVoucherDto _createVoucherModel = new();

        // Sự kiện báo cho trang cha biết để reload lại bảng sau khi Lưu thành công
        [Parameter]
        public EventCallback OnSuccess { get; set; }

        public void OpenForCreate()
        {
            _isEdit = false;
            _createVoucherModel = new CreateVoucherDto(); // Reset form rỗng
            _errorMessage = null;
            _isOpen = true;
            StateHasChanged();
        }

        public void OpenForEdit(VoucherDto dto)
        {
            _isEdit = true;
            _updateVoucherModel = new UpdateVoucherDto
            {
                VoucherId = dto.VoucherId,
                Code = dto.Code,
                Description = dto.Description,
                DiscountAmount = dto.DiscountAmount,
                DiscountPercent = dto.DiscountPercent,
                MinOrderAmount = dto.MinOrderAmount,
                MaxDiscountAmount = dto.MaxDiscountAmount,
                TotalUsageLimit = dto.TotalUsageLimit,
                ExpiredAt = dto.ExpiredAt,
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
                    var result = await ContentClientService.UpdateVoucherAsync(_updateVoucherModel);
                    if (!result.Success)
                    {
                        _errorMessage = result.Message ?? "Cập nhật mã giảm giá thất bại";
                        Toast.ShowError(_errorMessage);
                        return;
                    }
                }
                else
                {
                    var result = await ContentClientService.CreateVoucherAsync(_createVoucherModel);
                    if (!result.Success)
                    {
                        _errorMessage = result.Message ?? "Thêm mới mã giảm giá thất bại";
                        Toast.ShowError(_errorMessage);
                        return;
                    }
                }
                Close();
                Toast.ShowSuccess("Lưu mã giảm giá thành công");
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
