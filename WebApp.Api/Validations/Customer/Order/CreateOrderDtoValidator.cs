using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebBanSach.Shared.Dtos.CustomerDtos.Order;

namespace WebBanSach.Api.Validations.Customer.Order
{
    // CreateOrderDtoValidator.cs
    public class CreateOrderDtoValidator : AbstractValidator<CreateOrderDto>
    {
        public CreateOrderDtoValidator()
        {
            RuleFor(x => x.ReceiverName)
                .NotEmpty().WithMessage("Tên người nhận không được để trống")
                .MaximumLength(150);
            RuleFor(x => x.ReceiverPhone)
                .NotEmpty().WithMessage("Số điện thoại người nhận không được để trống")
                .Matches(@"^(0\d{9})$").WithMessage("Số điện thoại không hợp lệ");
            RuleFor(x => x.Province)
                .NotEmpty().WithMessage("Vui lòng chọn Tỉnh/Thành phố");
            RuleFor(x => x.District)
                .NotEmpty().WithMessage("Vui lòng chọn Quận/Huyện");
            RuleFor(x => x.Ward)
                .NotEmpty().WithMessage("Vui lòng chọn Phường/Xã");
            RuleFor(x => x.DetailAddress)
                .NotEmpty().WithMessage("Vui lòng nhập địa chỉ chi tiết")
                .MaximumLength(300);
            RuleFor(x => x.PaymentMethod)
                .InclusiveBetween(0, 3).WithMessage("Phương thức thanh toán không hợp lệ");
        }
    }
}
