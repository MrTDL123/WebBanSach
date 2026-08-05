using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebApp.Shared.Dtos.CustomerDtos.FreeBook;

namespace WebApp.Shared.Validations.Customer.FreeBook
{
    // CreateBookRequestDtoValidator.cs
    public class CreateBookRequestDtoValidator : AbstractValidator<CreateBookRequestDto>
    {
        public CreateBookRequestDtoValidator()
        {
            RuleFor(x => x.FreeBookId).GreaterThan(0).WithMessage("Vui lòng chọn sách muốn thỉnh");
            RuleFor(x => x.QuantityRequested)
                .GreaterThan(0).WithMessage("Số lượng phải lớn hơn 0")
                .LessThanOrEqualTo(3).WithMessage("Mỗi lần thỉnh tối đa 3 cuốn");
            RuleFor(x => x.Reason)
                .NotEmpty().WithMessage("Vui lòng điền lý do thỉnh sách")
                .MinimumLength(50).WithMessage("Lý do cần ít nhất 50 ký tự để nhân viên có thể xét duyệt")
                .MaximumLength(2000);
            RuleFor(x => x.ReceiverName)
                .NotEmpty().WithMessage("Tên người nhận không được để trống");
            RuleFor(x => x.ReceiverPhone)
                .NotEmpty().WithMessage("Số điện thoại không được để trống")
                .Matches(@"^(0\d{9})$").WithMessage("Số điện thoại không hợp lệ");
            RuleFor(x => x.ShippingAddress)
                .NotEmpty().WithMessage("Địa chỉ nhận sách không được để trống");
        }
    }
}
