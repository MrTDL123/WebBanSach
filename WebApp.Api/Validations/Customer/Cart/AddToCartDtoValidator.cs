using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebApp.Shared.Dtos.CustomerDtos.Cart;

namespace WebApp.Api.Validations.Customer.Cart
{
    // AddToCartDtoValidator.cs
    public class AddToCartDtoValidator : AbstractValidator<AddToCartDto>
    {
        public AddToCartDtoValidator()
        {
            RuleFor(x => x.ProductId)
                .GreaterThan(0).WithMessage("Mã sản phẩm không hợp lệ");
            RuleFor(x => x.Quantity)
                .GreaterThan(0).WithMessage("Số lượng phải lớn hơn 0")
                .LessThanOrEqualTo(100).WithMessage("Số lượng mỗi lần thêm tối đa 100");
        }
    }
}
