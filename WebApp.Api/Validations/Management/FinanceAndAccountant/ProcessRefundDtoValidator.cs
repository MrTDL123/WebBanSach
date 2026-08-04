using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebBanSach.Shared.Dtos.AdminClient.FinanceAndAccountant;

namespace WebBanSach.Api.Validations.Admin.FinanceAndAccountant
{
    public class ProcessRefundDtoValidator : AbstractValidator<ProcessRefundDto>
    {
        public ProcessRefundDtoValidator()
        {
            RuleFor(x => x.ReturnRequestId)
                .GreaterThan(0).WithMessage("Mã phiếu trả hàng không hợp lệ");

            RuleFor(x => x.RefundAmount)
                .GreaterThan(0).WithMessage("Số tiền hoàn trả phải lớn hơn 0");

            RuleFor(x => x.TransactionReference)
                .NotEmpty().WithMessage("Vui lòng nhập mã giao dịch ngân hàng hoàn tiền");
        }
    }
}
