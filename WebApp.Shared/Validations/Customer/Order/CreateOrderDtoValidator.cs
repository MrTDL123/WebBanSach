using FluentValidation;
using WebApp.Shared.Dtos.CustomerDtos.Order;

namespace WebApp.Shared.Validations.Customer.Order
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

            // Nếu không dùng SavedAddressId thì DetailAddress bắt buộc phải có
            When(x => !x.SavedAddressId.HasValue || x.SavedAddressId.Value <= 0, () =>
            {
                RuleFor(x => x.DetailAddress)
                    .NotEmpty().WithMessage("Vui lòng nhập địa chỉ chi tiết giao hàng")
                    .MaximumLength(300);
            });

            RuleFor(x => x.PaymentMethod)
                .InclusiveBetween(0, 3).WithMessage("Phương thức thanh toán không hợp lệ");
        }
    }
}
