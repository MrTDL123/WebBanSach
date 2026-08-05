using FluentValidation;
using WebApp.Shared.Dtos.AdminClient.ContentAndMarketing;

namespace WebApp.Api.Validations.Admin.ContentAndMarketing
{
    public class UpdateCategoryDtoValidator : AbstractValidator<UpdateCategoryDto>
    {
        public UpdateCategoryDtoValidator()
        {
            RuleFor(x => x.CategoryId)
                .GreaterThan(0).WithMessage("ID danh mục không hợp lệ");

            RuleFor(x => x.CategoryName)
                .NotEmpty().WithMessage("Tên danh mục không được để trống")
                .MaximumLength(150).WithMessage("Tên danh mục tối đa 150 ký tự");
        }
    }
}
