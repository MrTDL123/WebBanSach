using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebApp.Shared.Dtos.AdminClient.ContentAndMarketing;

namespace WebApp.Api.Validations.Admin.ContentAndMarketing
{
    public class CreateVoucherDtoValidator : AbstractValidator<CreateVoucherDto>
    {
        public CreateVoucherDtoValidator()
        {
            RuleFor(x => x.Code)
                .NotEmpty().WithMessage("Mã voucher không được để trống")
                .MaximumLength(50).WithMessage("Mã voucher tối đa 50 ký tự");

            RuleFor(x => x.MinOrderAmount)
                .GreaterThanOrEqualTo(0).WithMessage("Giá trị đơn hàng tối thiểu không được âm");

            RuleFor(x => x.TotalUsageLimit)
                .GreaterThan(0).WithMessage("Tổng lượt sử dụng phải lớn hơn 0");

            RuleFor(x => x.ExpiredAt)
                .GreaterThan(DateTime.UtcNow).WithMessage("Ngày hết hạn phải ở tương lai");
        }
    }
}
