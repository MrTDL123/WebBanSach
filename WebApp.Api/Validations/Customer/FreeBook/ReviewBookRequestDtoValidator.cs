using FluentValidation;
using WebBanSach.Shared.Dtos.AdminClient.CustomerSupport;

namespace WebBanSach.Api.Validations.Customer.FreeBook
{
    public class ReviewBookRequestDtoValidator : AbstractValidator<ReviewBookRequestDto>
    {
        public ReviewBookRequestDtoValidator()
        {
            RuleFor(x => x.RequestId).GreaterThan(0);

            When(x => !x.IsApproved, () =>
            {
                RuleFor(x => x.RejectedReason)
                    .NotEmpty().WithMessage("Vui lòng ghi rõ lý do từ chối để thông báo cho khách hàng")
                    .MinimumLength(20);
            });
        }
    }
}
