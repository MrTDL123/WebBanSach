using FluentValidation;
using WebBanSach.Shared.Dtos.AdminClient.CustomerSupport;
using WebBanSach.Shared.Dtos.AdminClient.ContentAndMarketing;
using WebBanSach.Shared.Dtos.Common;

namespace WebBanSach.Api.Validations.Common
{
    public class CreateSupportTicketDtoValidator : AbstractValidator<CreateSupportTicketDto>
    {
        public CreateSupportTicketDtoValidator()
        {
            RuleFor(x => x.Subject)
                .NotEmpty().WithMessage("Tiêu đề không được để trống")
                .MaximumLength(200).WithMessage("Tiêu đề tối đa 200 ký tự");

            RuleFor(x => x.Message)
                .NotEmpty().WithMessage("Nội dung không được để trống")
                .MinimumLength(20).WithMessage("Nội dung cần ít nhất 20 ký tự để hỗ trợ tốt nhất")
                .MaximumLength(3000);
        }
    }
}
