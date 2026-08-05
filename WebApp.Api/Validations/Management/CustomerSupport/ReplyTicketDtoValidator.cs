using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebApp.Shared.Dtos.AdminClient.CustomerSupport;

namespace WebApp.Api.Validations.Admin.CustomerSupport
{
    public class ReplyTicketDtoValidator : AbstractValidator<ReplyTicketDto>
    {
        public ReplyTicketDtoValidator()
        {
            RuleFor(x => x.TicketId)
                .GreaterThan(0).WithMessage("Mã ticket không hợp lệ");

            RuleFor(x => x.Content)
                .NotEmpty().WithMessage("Nội dung phản hồi không được để trống")
                .MaximumLength(3000);
        }
    }
}
