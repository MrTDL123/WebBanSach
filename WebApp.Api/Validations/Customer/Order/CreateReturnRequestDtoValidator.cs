using FluentValidation;
using WebApp.Shared.Dtos.Customer.Order;

namespace WebApp.Api.Validations.Customer.Order
{
    public class CreateReturnRequestDtoValidator : AbstractValidator<CreateReturnRequestDto>
    {
        private const int MaxBase64ImageLength = 7 * 1024 * 1024;

        public CreateReturnRequestDtoValidator()
        {
            RuleFor(x => x.OrderId)
                .GreaterThan(0).WithMessage("Mã đơn hàng không hợp lệ");

            RuleFor(x => x.Reason)
                .NotEmpty().WithMessage("Vui lòng nhập lý do yêu cầu trả hàng")
                .MinimumLength(10).WithMessage("Lý do trả hàng cần ít nhất 10 ký tự")
                .MaximumLength(500);

            // Validation kiểm tra ảnh minh chứng (Max 5MB mỗi ảnh)
            When(x => x.EvidenceImagesBase64 != null && x.EvidenceImagesBase64.Count > 0, () =>
            {
                RuleForEach(x => x.EvidenceImagesBase64)
                    .Must(base64 => string.IsNullOrEmpty(base64) || base64.Length <= MaxBase64ImageLength)
                    .WithMessage("Mỗi ảnh minh chứng không được vượt quá 5MB");
            });
        }
    }
}
