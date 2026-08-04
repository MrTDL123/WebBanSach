using FluentValidation;
using WebBanSach.Shared.Dtos.Management.ContentAndMarketing;

namespace WebBanSach.Api.Validations.Management.ContentAndMarketing
{
    public class UpdateProductDtoValidator : AbstractValidator<UpdateProductDto>
    {
        public UpdateProductDtoValidator()
        {
            RuleFor(x => x.ProductId)
                .GreaterThan(0).WithMessage("Mã sản phẩm không hợp lệ");

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
        }
    }
}
