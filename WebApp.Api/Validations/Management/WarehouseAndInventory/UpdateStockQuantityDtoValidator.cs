using FluentValidation;
using WebBanSach.Shared.Dtos.AdminClient.WarehouseAndInventory;

namespace WebBanSach.Api.Validations.Admin.WarehouseAndInventory
{
    public class UpdateStockQuantityDtoValidator : AbstractValidator<UpdateStockQuantityDto>
    {
        public UpdateStockQuantityDtoValidator()
        {
            RuleFor(x => x.ProductId)
                .GreaterThan(0).WithMessage("Mã sản phẩm không hợp lệ");

            RuleFor(x => x.NewQuantity)
                .GreaterThanOrEqualTo(0).WithMessage("Số lượng tồn kho mới không được âm");

            RuleFor(x => x.Reason)
                .NotEmpty().WithMessage("Vui lòng ghi rõ lý do thay đổi tồn kho");
        }
    }
}
