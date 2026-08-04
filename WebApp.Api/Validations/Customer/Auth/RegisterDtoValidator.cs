using FluentValidation;
using WebBanSach.Shared.Dtos.Customer.Auth;

namespace WebBanSach.Api.Validations.Customer.Auth
{
    public class RegisterDtoValidator : AbstractValidator<RegisterDto>
    {
        public RegisterDtoValidator()
        {
            RuleFor(x => x.FullName)
                .NotEmpty().WithMessage("Họ tên không được để trống")
                .MaximumLength(150).WithMessage("Họ tên tối đa 150 ký tự");

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email không được để trống")
                .EmailAddress().WithMessage("Định dạng Email không hợp lệ");

            RuleFor(x => x.PhoneNumber)
                .NotEmpty().WithMessage("Số điện thoại không được để trống")
                .Matches(@"^(0\d{9})$").WithMessage("Số điện thoại không hợp lệ (cần 10 chữ số, bắt đầu bằng 0)");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Mật khẩu không được để trống")
                .MinimumLength(6).WithMessage("Mật khẩu phải có ít nhất 6 ký tự");

            RuleFor(x => x.ConfirmPassword)
                .NotEmpty().WithMessage("Vui lòng xác nhận mật khẩu")
                .Equal(x => x.Password).WithMessage("Mật khẩu xác nhận không khớp");

            RuleFor(x => x.OtpCode)
                .NotEmpty().WithMessage("Vui lòng nhập mã OTP");
        }
    }
}
