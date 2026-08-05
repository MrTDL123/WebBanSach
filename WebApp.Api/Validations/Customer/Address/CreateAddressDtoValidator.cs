using FluentValidation;
using WebApp.Shared.Dtos.CustomerDtos.Address;

namespace WebApp.Api.Validations.Customer.Address
{
    public class CreateAddressDtoValidator : AbstractValidator<CreateAddressDto>
    {
        public CreateAddressDtoValidator()
        {
            RuleFor(x => x.ReceiverName)
                .NotEmpty().WithMessage("Họ tên người nhận không được để trống")
                .MaximumLength(150).WithMessage("Tên người nhận tối đa 150 ký tự");

            RuleFor(x => x.ReceiverPhone)
                .NotEmpty().WithMessage("Số điện thoại không được để trống")
                .Matches(@"^(0\d{9})$").WithMessage("Số điện thoại không hợp lệ (cần 10 chữ số, bắt đầu bằng 0)");

            RuleFor(x => x.Province)
                .NotEmpty().WithMessage("Vui lòng chọn Tỉnh/Thành phố");

            RuleFor(x => x.District)
                .NotEmpty().WithMessage("Vui lòng chọn Quận/Huyện");

            RuleFor(x => x.Ward)
                .NotEmpty().WithMessage("Vui lòng chọn Phường/Xã");

            RuleFor(x => x.DetailAddress)
                .NotEmpty().WithMessage("Địa chỉ chi tiết không được để trống")
                .MaximumLength(300).WithMessage("Địa chỉ chi tiết tối đa 300 ký tự");
        }
    }
}
