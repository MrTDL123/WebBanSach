using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebBanSach.Shared.Dtos.User;

namespace WebBanSach.Api.Validations.User
{
    public class ChangeEmailDtoValidator : AbstractValidator<ChangeEmailDto>
    {
        public ChangeEmailDtoValidator()
        {
            RuleFor(x => x.NewEmail)
                .NotEmpty().WithMessage("Email mới không được để trống")
                .EmailAddress().WithMessage("Email mới không đúng định dạng");

            RuleFor(x => x.OtpCode)
                .NotEmpty().WithMessage("Mã OTP không được để trống")
                .Matches(@"^\d{6}$").WithMessage("Mã OTP phải gồm 6 chữ số");
        }
    }
}
