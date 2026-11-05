using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebApp.Shared.Dtos.Management.ContentAndMarketing;

namespace WebApp.Shared.Validations.Admin.ContentAndMarketing
{
    public class CreateVoucherDtoValidator : AbstractValidator<CreateVoucherDto>
    {
        public CreateVoucherDtoValidator()
        {
            RuleFor(x => x.Code)
                .NotEmpty().WithMessage("Mã voucher không được để trống.")
                .MaximumLength(50).WithMessage("Mã voucher không được vượt quá 50 ký tự.");
            RuleFor(x => x.MinOrderAmount)
                .GreaterThanOrEqualTo(0).WithMessage("Giá trị đơn hàng tối thiểu không được âm.");
            RuleFor(x => x.TotalUsageLimit)
                .GreaterThan(0).WithMessage("Tổng lượt sử dụng phải lớn hơn 0.");
            RuleFor(x => x.ExpiredAt)
                .GreaterThan(DateTime.Now).WithMessage("Ngày hết hạn phải lớn hơn ngày hiện tại.");
            RuleFor(x => x)
                .Must(x => x.DiscountAmount > 0 || (x.DiscountPercent > 0 && x.DiscountPercent <= 100))
                .WithMessage("Voucher phải có mức giảm tiền (> 0) hoặc mức giảm phần trăm (từ 1% đến 100%).");
        }
    }
}
