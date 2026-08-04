using FluentValidation;
using WebBanSach.Shared.Dtos.Customer.Auth;

namespace WebBanSach.Api.Validations.Customer.Auth
{
    public class CustomerLoginDtoValidator : AbstractValidator<CustomerLoginDto>
    {
        public CustomerLoginDtoValidator()
        {
            RuleFor(x => x.EmailOrPhone)
                .NotEmpty().WithMessage("Vui lòng nhập Email hoặc Số điện thoại");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Mật khẩu không được để trống");
        }
    }
}
