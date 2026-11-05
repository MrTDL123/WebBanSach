using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;
using WebApp.Shared.Dtos.Management.ContentAndMarketing;

namespace WebApp.Shared.Validations.Management.ContentAndMarketing
{
    public class UpdateVoucherDtoValidator : AbstractValidator<UpdateVoucherDto>
    {
        public UpdateVoucherDtoValidator()
        {
            RuleFor(v => v.VoucherId)
                .GreaterThan(0).WithMessage("ID voucher không hợp lệ.");
            RuleFor(v => v.Code)
                .NotEmpty().WithMessage("Mã voucher không được để trống.")
                .MaximumLength(50).WithMessage("Mã voucher không được vượt quá 50 ký tự.");
            RuleFor(v => v.MinOrderAmount)
                .GreaterThanOrEqualTo(0).WithMessage("Giá trị đơn hàng tối thiểu không được âm.");
            RuleFor(v => v.TotalUsageLimit)
                .GreaterThan(0).WithMessage("Tổng lượt sử dụng phải lớn hơn 0.");
            RuleFor(v => v.ExpiredAt)
                .NotEmpty().WithMessage("Ngày hết hạn không được để trống.");
            RuleFor(v => v)
                .Must(v => v.DiscountAmount > 0 || (v.DiscountPercent > 0 && v.DiscountPercent <= 100))
                .WithMessage("Voucher phải có mức giảm tiền (> 0) hoặc mức giảm phần trăm (từ 1% đến 100%).");
        }
    }
}
