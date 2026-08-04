using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebBanSach.Shared.Dtos.AdminClient.OrderAndLogistics;

namespace WebBanSach.Api.Validations.Admin.OrderAndLogistics
{
    public class PushTo3PLDtoValidator : AbstractValidator<PushTo3PLDto>
    {
        public PushTo3PLDtoValidator()
        {
            RuleFor(x => x.OrderId)
                .GreaterThan(0).WithMessage("Mã đơn hàng không hợp lệ");

            RuleFor(x => x.ShippingProviderId)
                .GreaterThan(0).WithMessage("Vui lòng chọn đơn vị vận chuyển 3PL (GHN, GHTK...)");

            RuleFor(x => x.TotalWeightGram)
                .GreaterThan(0).WithMessage("Khối lượng đơn hàng phải lớn hơn 0g");
        }
    }
}
