using FluentValidation;
using WebApp.Shared.Dtos.Management.Auth;

namespace WebApp.Shared.Validations.Management.Auth
{
    public class AdminLoginDtoValidator : AbstractValidator<AdminLoginDto>
    {
        public AdminLoginDtoValidator()
        {
            RuleFor(x => x.UsernameOrEmail)
                .NotEmpty().WithMessage("Vui lòng nhập Tên đăng nhập hoặc Email quản trị");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Mật khẩu không được để trống");
        }
    }
}
