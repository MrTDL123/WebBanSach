using FluentValidation;
using WebApp.Shared.Dtos.AdminClient.OrderAndLogistics;

namespace WebApp.Shared.Validations.Admin.OrderAndLogistics
{
    public class UpdateOrderStatusDtoValidator : AbstractValidator<UpdateOrderStatusDto>
    {
        public UpdateOrderStatusDtoValidator()
        {
            RuleFor(x => x.OrderId).GreaterThan(0);
            RuleFor(x => x.NewStatus).InclusiveBetween(0, 5);

            When(x => x.NewStatus == 2, () =>
            {
                RuleFor(x => x.TrackingNumber)
                    .NotEmpty().WithMessage("Vui lòng nhập mã vận đơn khi giao hàng");
                RuleFor(x => x.ShippingProviderId)
                    .NotNull().WithMessage("Vui lòng chọn đơn vị vận chuyển");
            });
        }
    }
}
