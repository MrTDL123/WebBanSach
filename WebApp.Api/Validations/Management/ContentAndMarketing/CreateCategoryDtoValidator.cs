using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebBanSach.Shared.Dtos.AdminClient.ContentAndMarketing;

namespace WebBanSach.Api.Validations.Admin.ContentAndMarketing
{
    // CreateCategoryDtoValidator.cs
    public class CreateCategoryDtoValidator : AbstractValidator<CreateCategoryDto>
    {
        public CreateCategoryDtoValidator()
        {
            RuleFor(x => x.CategoryName)
                .NotEmpty().WithMessage("Tên danh mục không được để trống")
                .MaximumLength(150).WithMessage("Tên danh mục tối đa 150 ký tự");
        }
    }
}
