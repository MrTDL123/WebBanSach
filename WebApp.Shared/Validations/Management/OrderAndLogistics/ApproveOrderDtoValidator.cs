using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebApp.Shared.Dtos.AdminClient.OrderAndLogistics;

namespace WebApp.Shared.Validations.Admin.OrderAndLogistics
{
    public class ApproveOrderDtoValidator : AbstractValidator<ApproveOrderDto>
    {
        public ApproveOrderDtoValidator()
        {
            RuleFor(x => x.OrderId)
                .GreaterThan(0).WithMessage("Mã đơn hàng không hợp lệ");
        }
    }
}
