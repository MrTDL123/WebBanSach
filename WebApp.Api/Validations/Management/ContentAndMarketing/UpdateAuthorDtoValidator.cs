using FluentValidation;
using WebBanSach.Shared.Dtos.AdminClient.ContentAndMarketing;

namespace WebBanSach.Api.Validations.Admin.ContentAndMarketing
{
    public class UpdateAuthorDtoValidator : AbstractValidator<UpdateAuthorDto>
    {
        public UpdateAuthorDtoValidator()
        {
            RuleFor(x => x.AuthorId)
                .GreaterThan(0).WithMessage("ID tác giả không hợp lệ");

            RuleFor(x => x.AuthorName)
                .NotEmpty().WithMessage("Tên tác giả không được để trống")
                .MaximumLength(150).WithMessage("Tên tác giả tối đa 150 ký tự");

            RuleFor(x => x.Biography)
                .MaximumLength(2000).WithMessage("Tiểu sử tối đa 2000 ký tự")
                .When(x => x.Biography != null);
        }
    }
}
