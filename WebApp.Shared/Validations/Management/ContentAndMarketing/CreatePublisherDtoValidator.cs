using FluentValidation;
using WebApp.Shared.Dtos.AdminClient.ContentAndMarketing;

namespace WebApp.Shared.Validations.Admin.ContentAndMarketing
{
    public class CreatePublisherDtoValidator : AbstractValidator<CreatePublisherDto>
    {
        public CreatePublisherDtoValidator()
        {
            RuleFor(x => x.PublisherName)
                .NotEmpty().WithMessage("Tên nhà xuất bản không được để trống")
                .MaximumLength(150).WithMessage("Tên nhà xuất bản tối đa 150 ký tự");

            RuleFor(x => x.PhoneNumber)
                .Matches(@"^(0\d{9})$").WithMessage("Số điện thoại NXB không hợp lệ")
                .When(x => !string.IsNullOrEmpty(x.PhoneNumber));

            RuleFor(x => x.Email)
                .EmailAddress().WithMessage("Email NXB không hợp lệ")
                .When(x => !string.IsNullOrEmpty(x.Email));
        }
    }
}
