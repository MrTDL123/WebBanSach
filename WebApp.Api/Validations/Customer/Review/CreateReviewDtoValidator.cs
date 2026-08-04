using FluentValidation;
using WebBanSach.Shared.Dtos.CustomerDtos.Review;

namespace WebBanSach.Api.Validations.Customer.Review
{
    public class CreateReviewDtoValidator : AbstractValidator<CreateReviewDto>
    {
        public CreateReviewDtoValidator()
        {
            RuleFor(x => x.ProductId)
                .GreaterThan(0).WithMessage("Mã sản phẩm không hợp lệ");

            RuleFor(x => x.OrderId)
                .GreaterThan(0).WithMessage("Mã đơn hàng không hợp lệ");

            RuleFor(x => x.Rating)
                .InclusiveBetween(1, 5).WithMessage("Đánh giá sao phải từ 1 đến 5 sao");

            RuleFor(x => x.Comment)
                .MaximumLength(1000).WithMessage("Nội dung nhận xét tối đa 1000 ký tự")
                .When(x => !string.IsNullOrEmpty(x.Comment));
        }
    }
}
