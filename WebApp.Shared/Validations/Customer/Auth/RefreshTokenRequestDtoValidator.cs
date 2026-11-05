using FluentValidation;
using WebApp.Shared.Dtos.Customer.Auth;

namespace WebApp.Shared.Validations.Customer.Auth
{
    public class RefreshTokenRequestDtoValidator : AbstractValidator<RefreshTokenRequestDto>
    {
        public RefreshTokenRequestDtoValidator()
        {
            RuleFor(x => x.AccessToken)
                .NotEmpty().WithMessage("Access Token không được để trống.");

            RuleFor(x => x.RefreshToken)
                .NotEmpty().WithMessage("Refresh Token không được để trống.");
        }
    }
}
