using FluentValidation;
using WebApp.Shared.Dtos.Common;

namespace WebApp.Api.Validations.Common
{
    public class BasePagedQueryDtoValidator : AbstractValidator<BasePagedQueryDto>
    {
        public BasePagedQueryDtoValidator()
        {
            RuleFor(x => x.PageIndex)
                .GreaterThanOrEqualTo(1).WithMessage("Trang hiện tại phải từ trang 1 trở lên");

            RuleFor(x => x.PageSize)
                .InclusiveBetween(1, 100).WithMessage("Kích thước mỗi trang phải từ 1 đến 100 phần tử");
        }
    }
}
