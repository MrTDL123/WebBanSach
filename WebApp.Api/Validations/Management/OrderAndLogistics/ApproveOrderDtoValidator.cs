using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebBanSach.Shared.Dtos.AdminClient.OrderAndLogistics;

namespace WebBanSach.Api.Validations.Admin.OrderAndLogistics
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
