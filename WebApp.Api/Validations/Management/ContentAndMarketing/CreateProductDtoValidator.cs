using FluentValidation;
using WebBanSach.Shared.Dtos.Management.ContentAndMarketing;

namespace WebBanSach.Api.Validations.Management.ContentAndMarketing
{
    public class CreateProductDtoValidator : AbstractValidator<CreateProductDto>
    {
        // 5MB trong Base64 tương đương khoảng 7,000,000 ký tự
        private const int MaxBase64ImageLength = 7 * 1024 * 1024;

        public CreateProductDtoValidator()
        {
            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("Tên sách không được để trống")
                .MaximumLength(255).WithMessage("Tên sách tối đa 255 ký tự");

            RuleFor(x => x.Price)
                .GreaterThan(0).WithMessage("Giá bán phải lớn hơn 0");

            RuleFor(x => x.DiscountPercent)
                .InclusiveBetween(0, 100).WithMessage("Phần trăm giảm giá phải từ 0 đến 100");

            RuleFor(x => x.StockQuantity)
                .GreaterThanOrEqualTo(0).WithMessage("Số lượng tồn kho không được âm");

            RuleFor(x => x.CategoryId)
                .GreaterThan(0).WithMessage("Vui lòng chọn danh mục sách");

            RuleFor(x => x.AuthorId)
                .GreaterThan(0).WithMessage("Vui lòng chọn tác giả");

            RuleFor(x => x.PublisherId)
                .GreaterThan(0).WithMessage("Vui lòng chọn nhà xuất bản");

            // Validation kiểm tra dung lượng ảnh upload (Max 5MB)
            When(x => !string.IsNullOrEmpty(x.MainImageBase64), () =>
            {
                RuleFor(x => x.MainImageBase64)
                    .Must(base64 => base64!.Length <= MaxBase64ImageLength)
                    .WithMessage("Kích thước ảnh bìa chính không được vượt quá 5MB");
            });
        }
    }
}
