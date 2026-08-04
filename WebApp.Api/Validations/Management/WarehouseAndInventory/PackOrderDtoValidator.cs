using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebBanSach.Shared.Dtos.AdminClient.WarehouseAndInventory;

namespace WebBanSach.Api.Validations.Admin.WarehouseAndInventory
{
    public class PackOrderDtoValidator : AbstractValidator<PackOrderDto>
    {
        public PackOrderDtoValidator()
        {
            RuleFor(x => x.OrderId)
                .GreaterThan(0).WithMessage("Mã đơn hàng không hợp lệ");

            RuleFor(x => x.WarehouseStaffId)
                .NotEmpty().WithMessage("Mã nhân viên kho không được để trống");
        }
    }
}
