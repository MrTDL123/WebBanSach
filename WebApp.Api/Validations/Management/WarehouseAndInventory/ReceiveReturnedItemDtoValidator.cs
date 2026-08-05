using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebApp.Shared.Dtos.AdminClient.WarehouseAndInventory;

namespace WebApp.Api.Validations.Admin.WarehouseAndInventory
{
    public class ReceiveReturnedItemDtoValidator : AbstractValidator<ReceiveReturnedItemDto>
    {
        public ReceiveReturnedItemDtoValidator()
        {
            RuleFor(x => x.ReturnRequestId)
                .GreaterThan(0).WithMessage("Mã phiếu trả hàng không hợp lệ");

            RuleFor(x => x.OrderId)
                .GreaterThan(0).WithMessage("Mã đơn hàng không hợp lệ");

            RuleFor(x => x.RestockQuantity)
                .GreaterThanOrEqualTo(0).WithMessage("Số lượng nhập lại kho không được âm");
        }
    }
}
