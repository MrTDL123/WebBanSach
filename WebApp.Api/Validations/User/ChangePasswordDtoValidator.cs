using FluentValidation;
using WebBanSach.Shared.Dtos.User;

namespace WebBanSach.Api.Validations.User
{
    public class ChangePasswordDtoValidator : AbstractValidator<ChangePasswordDto>
    {
        public ChangePasswordDtoValidator()
        {
            RuleFor(x => x.CurrentPassword)
                .NotEmpty().WithMessage("Vui lòng nhập mật khẩu hiện tại");

            RuleFor(x => x.NewPassword)
                .NotEmpty().WithMessage("Vui lòng nhập mật khẩu mới")
                .MinimumLength(6).WithMessage("Mật khẩu mới phải có ít nhất 6 ký tự")
                .NotEqual(x => x.CurrentPassword).WithMessage("Mật khẩu mới không được trùng với mật khẩu cũ");

            RuleFor(x => x.ConfirmNewPassword)
                .NotEmpty().WithMessage("Vui lòng xác nhận mật khẩu mới")
                .Equal(x => x.NewPassword).WithMessage("Mật khẩu xác nhận không khớp");
        }
    }
}
