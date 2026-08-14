using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebBanSach.Shared.Dtos.User;

namespace WebBanSach.Api.Validations.User
{
    public class ChangePhoneNumberDtoValidator : AbstractValidator<ChangePhoneNumberDto>
    {
        public ChangePhoneNumberDtoValidator()
        {
            RuleFor(x => x.NewPhoneNumber)
                .NotEmpty().WithMessage("Số điện thoại mới không được để trống")
                .Matches(@"^(0\d{9})$").WithMessage("Số điện thoại mới không hợp lệ (cần 10 chữ số, bắt đầu bằng 0)");

            RuleFor(x => x.OtpCode)
                .NotEmpty().WithMessage("Mã OTP không được để trống")
                .Matches(@"^\d{6}$").WithMessage("Mã OTP phải gồm 6 chữ số");
        }
    }
}
