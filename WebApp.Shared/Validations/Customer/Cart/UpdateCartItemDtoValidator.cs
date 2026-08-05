using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebApp.Shared.Dtos.CustomerDtos.Cart;

namespace WebApp.Shared.Validations.Customer.Cart
{

    // UpdateCartItemDtoValidator.cs
    public class UpdateCartItemDtoValidator : AbstractValidator<UpdateCartItemDto>
    {
        public UpdateCartItemDtoValidator()
        {
            RuleFor(x => x.ProductId).GreaterThan(0);
            RuleFor(x => x.Quantity)
                .GreaterThanOrEqualTo(0).WithMessage("Số lượng không được âm")
                .LessThanOrEqualTo(100);
        }
    }
}
